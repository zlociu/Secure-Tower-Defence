import zipfile
from datetime import datetime
import os
from os import listdir
from os.path import isfile, join

from django.db.utils import IntegrityError
from django.shortcuts import render
from django.http import JsonResponse, HttpResponse, HttpResponseNotFound
from django.http import FileResponse
from django.views.decorators.csrf import csrf_exempt
# Create your views here.
from ServerSide.game_models import Mob
from ServerSide.setter_models import (
    Test,
    User,
    Player,
    KeysRegister
)
from ServerSide.graphics_models import (
    Graphic,
    Music,
    Other,
)
from ServerSide.game_models import Map


def setup(request):
    """
    url: /setup
    :param request: GET
    Run to setup database with default data
    """

    try:
        Test.objects.create(actual_build=0)

        User.objects.create(identity="test_user_1",
                            login="test_user",
                            password="admin",
                            game_build=0,
                            public_key="public_key",
                            private_key="private_key"
                            )

        user = User.objects.get(identity="test_user_1")

        Player.objects.create(identity=user,
                              user_address="user_address_1",
                              player_address="player_address_1",
                              level=0,
                              points=0
                              )

        player = Player.objects.get(user_address="user_address_1")

        KeysRegister.objects.create(
            user_address=player,
            public_key="public_key"
        )

        Mob.objects.create(
            name="mob",
            type=1,
            attack_damage=10,
            attack_range=10.0,
            fire_rate=2.0,
            health=100,
            speed=23.3,
            level=9,
            points_remaining=24,
            spawn_time=22)

        response = JsonResponse({"udane": "setup"})
    except:
        response = JsonResponse({"udane": "nie"})

    return response


@csrf_exempt
def login(request):
    """
    url: "/login"

    :param request: POST{"login": login, "pass": password}
    :return:
    {"login": Accepted} if login successful, status code: 200
    {"login": Declined} if login unsuccessful, status code: 403
    """
    login = request.POST["login"]
    password = request.POST["pass"]

    usr = User.objects.filter(login=login, password=password)

    print(usr.all())

    if usr:
        response = JsonResponse({"login": "Accepted"})
        response.status_code = 200
    else:
        response = JsonResponse({"login": "Declined"})
        response.status_code = 403

    return response


@csrf_exempt
def map_upload(request):
    """
    url: /map_upload

    map_array - map array written as positive integers, separated with ";".
    player_address - field in Player object.
    :param request: POST {"map": map_array, "player_address": player_address}
    :return:
    {"status": "Map successfully loaded"}, status code: 200
    {"status": "Error loading map"}, status code: 404
    """
    map_arr = request.POST["map"]
    player_address = request.POST["player_address"]

    try:
        player = Player.objects.get(player_address=player_address)

        print(map_arr)

        Map.objects.get_or_create(
            map_address="random_address",
            player_address=player,
            map_array=map_arr,
            validationTimeFrom=datetime.now())

        response = JsonResponse({"status": "Map successfully loaded"})
        response.status_code = 200
    except:
        response = JsonResponse({"status": "Error loading map"})
        response.status_code = 404

    return response


@csrf_exempt
def map_download(request, map_id):
    """
    map_addr - Map identifier. Field in Map object.

    url: /map_download/{map_addr}

    :param request: GET
    :param map_id: map_addr
    :return:
    {"map": map_obj.map_array}, status code 200
    {"map": "Failed"}, status code 403
    """
    try:
        map_obj = Map.objects.get(map_address=map_id)
        response = JsonResponse({"map": map_obj.map_array})
        response.status_code = 200
    except:
        response = JsonResponse({"map": "Failed"})
        response.status_code = 403

    return response


def serve_new_instance(request):
    """
    Used for new user, that wants to download game.

    url: /download_full_game

    :param request: GET
    :return:
    Stream containing zipfile with all game files including graphic and music.
    """
    files = Graphic.objects.all()

    filenames = []
    zipfile_name = "zip_z_plikami"

    for f in files.all():
        filenames.append(f.path)

    print(filenames)

    response = HttpResponse(content_type='application/zip')
    zip_file = zipfile.ZipFile(response, 'w')
    for filename in filenames:
        zip_file.write(filename)
    response['Content-Disposition'] = f'attachment; filename={zipfile_name}'
    return response


def submit_update(request):
    """
    ! Used only by administrator.
    Loading all files from path to database.
    {path} = "TowerDefense\\Packages"
    Method, that updates game files.

    url: /submit_update

    :param request: GET
    ver = version number : positive integer
    :return:
    {"update": "ok"}, status code 200
    """
    t = Test.objects.get(name="game")
    t.actual_build = t.actual_build + 1
    ver = t.actual_build
    t.save()

    thisdir = "TowerDefense\\Packages"

    graphic_files = []
    music_files = []
    other_files = []

    ### List all files

    for r, d, f in os.walk(thisdir):
        for file in f:
            if file.endswith(".png"):
                graphic_files.append(os.path.join(r, file))
            elif file.endswith(".ogg"):
                music_files.append(os.path.join(r, file))
            else:
                other_files.append(os.path.join(r, file))

    ### Clean undesired files

    g_all = Graphic.objects.all()
    m_all = Music.objects.all()
    o_all = Other.objects.all()

    for g in g_all:
        if g.path not in graphic_files:
            Graphic.objects.get(path=g.path).delete()

    for m in m_all:
        if m.path not in music_files:
            Music.objects.get(path=m.path).delete()

    for o in o_all:
        if o.path not in other_files:
            Other.objects.get(path=o.path).delete()

    ### Update database

    for f in graphic_files:
        try:
            print(f)
            g, c = Graphic.objects.get_or_create(path=f, build=ver)
            if c:
                g.version = ver
        except IntegrityError:
            print("passing")
            pass

    for f in music_files:
        try:
            m, c = Music.objects.get_or_create(path=f, build=ver)
            if c:
                m.version = ver
        except IntegrityError:
            pass

    for f in other_files:
        try:
            o, c = Other.objects.get_or_create(path=f, build=ver)
            if c:
                o.version = ver
        except IntegrityError:
            pass

    response = JsonResponse({"update": "ok"})
    response.status_code = 200

    return response


def serve_newest_update(request, user_identity):
    """
    user_identity - unique user id.
    System for updating game files.
    url: /request_update/user_identity
    :param request: GET
    :param user_identity:
    :return:
    """

    user = User.objects.get(identity=user_identity)
    user_build = user.game_build

    actual_build = Test.objects.get(name="game").actual_build

    if user_build == actual_build:
        response = JsonResponse({"status": "aktualne"})
        response.status_code = 200

    else:
        files = []

        all_graphic = Graphic.objects.all()
        all_music = Music.objects.all()
        all_other = Other.objects.all()

        zip_name = f"update_{actual_build}"

        [files.append(b.path) for b in all_graphic if b.build > user_build]
        [files.append(b.path) for b in all_music if b.build > user_build]
        [files.append(b.path) for b in all_other if b.build > user_build]

        print(files)

        response = HttpResponse(content_type='application/zip')

        response.set_cookie("build", actual_build)

        zip_file = zipfile.ZipFile(response, 'w')
        for filename in files:
            zip_file.write(filename)
        response['Content-Disposition'] = f'attachment; filename={zip_name}'

        print(response.items())

        user.game_build = actual_build
        user.save()

    return response
