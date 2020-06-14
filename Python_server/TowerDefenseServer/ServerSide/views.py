import json
import os
import re
import zipfile
from datetime import datetime
from pathlib import Path

import django
from django.contrib.auth import authenticate, logout, login
from django.db.utils import IntegrityError
from django.http import JsonResponse, HttpResponse
from django.views.decorators.csrf import csrf_exempt

from ServerSide.models.game_models import Map
from ServerSide.models.game_models import Mob
from ServerSide.models.graphics_models import Graphic, Music, Other
from ServerSide.models.player_models import Player
from ServerSide.models.setter_models import Test
from ServerSide.models.tower_models import Tower
from ServerSide.models.user_models import MyUser


@csrf_exempt
def register(request):
    username = request.POST['username']
    passwd = request.POST['password']

    print(username)
    print(passwd)

    game_build = Test.objects.get(name="game").actual_build

    try:
        MyUser.objects.create_user(username=username, password=passwd, game_build=game_build)
        response = JsonResponse({"User": "Created"})
        response.status_code = 201
        print("register OK")
    except django.db.utils.IntegrityError:
        response = JsonResponse({"User": "Already exists"})
        response.status_code = 409

    return response


@csrf_exempt
def login_user(request):
    username = request.POST['username']
    password = request.POST['password']
    user = authenticate(request, username=username, password=password)
    if user is not None:
        login(request, user)
        response = JsonResponse({"User": "Logged"})
        response.status_code = 201
    else:
        response = JsonResponse({"User": "Invalid login"})
        response.status_code = 401

    return response


def logout_view(request):
    logout(request)


# noinspection PyBroadException
def setup(request):
    """
    url: /setup
    :param request: GET
    Run to setup database with default data
    """

    try:
        Test.objects.create(actual_build=0)

        MyUser.objects.create(username="test_user_1", password="admin", game_build=0)

        user = MyUser.objects.get(username="test_user_1")

        Player.objects.create(identity=user,
                              user_address="user_address_1",
                              player_address="player_address_1",
                              level=0,
                              points=0
                              )

        Player.objects.get(user_address="user_address_1")

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

        response = JsonResponse({"success": True})
    except Exception:
        response = JsonResponse({"success": False})

    return response


def update_stats(request):
    identity = request.POST['player']
    new_level = request.POST['level']

    status = Player.objects.filter(identity=identity).update(level=new_level)

    if status:
        response = JsonResponse({"Player": "Error"})
        response.status_code = 403
    else:
        response = JsonResponse({"Player": f"new level: {new_level}"})
        response.status_code = 200

    return response


def create_tower(request):
    identity = request.POST['identity']
    name = request.POST['name']
    tower_type = request.POST['type']
    level = request.POST['level']
    attack_damage = request.POST['attack_damage']
    attack_distance = request.POST['attack_distance']
    fire_rate = request.POST['fire_rate']
    price = request.POST['price']

    is_created = Tower.objects.get_or_create(
        identity=identity,
        name=name,
        type=tower_type,
        level=level,
        attack_damage=attack_damage,
        attack_distance=attack_distance,
        fire_rate=fire_rate,
        price=price
    )

    if is_created:
        response = JsonResponse({"Tower": "Already exists"})
        response.status_code = 403
    else:
        response = JsonResponse({"Tower": "Created"})
        response.status_code = 200

    return response


def create_player(request):
    """
    Constructor
    # id = unique number, will be public key
    # login = user's name
    # password = password hashed with SHA256
    """

    identity = request.POST['identity']
    player_address = request.POST['player_address']
    user_address = request.POST['user_address']

    Player.objects.get_or_create(
        identity=identity,
        player_address=player_address,
        user_address=user_address
    )


def create_mob(request):
    name = request.POST['name']
    _type = request.POST['type']
    attack_damage = request.POST['attack_damage']
    attack_range = request.POST['attack_range']
    fire_rate = request.POST['fire_rate']
    health = request.POST['health']
    speed = request.POST['speed']
    level = request.POST['level']
    points_remaining = request.POST['points_remaining']
    spawn_time = request.POST['spawn_time']

    is_created = Mob.objects.get_or_create(
        name=name,
        type=_type,
        attack_damage=attack_damage,
        attack_range=attack_range,
        fire_rate=fire_rate,
        health=health,
        speed=speed,
        level=level,
        points_remaining=points_remaining,
        spawn_time=spawn_time
    )

    if is_created:
        response = JsonResponse({"Mob": "Already exists"})
        response.status_code = 403
    else:
        response = JsonResponse({"Mob": "Created"})
        response.status_code = 200

    return response


# noinspection PyBroadException
@csrf_exempt
def map_upload(request):
    """
    url: /map_upload

    map_array - map array written as positive integers, separated with ";".
    player_address - field in Player object.
    :param request: POST {"map": map_array, "path": player_address}
    :return:
    {"status": "map successfully loaded"}, status code: 201
    {"status": "error loading map"}, status code: 400
    """
    content_json = json.loads(request.body)
    map_array = content_json["map"]
    map_path: str = content_json["path"]

    try:
        print(map_array)

        if map_path.endswith(".json"):
            map_path -= ".json"

        Path("resources/maps/").mkdir(parents=True, exist_ok=True)
        with open(f"resources/maps/{map_path}.json", "w+") as f:
            f.write(json.dumps({'map': map_array}))

        Map.objects.update_or_create(path=map_path, validationTimeFrom=datetime.now())

        response = JsonResponse({"status": "map successfully loaded"})
        response.status_code = 201
    except Exception:
        response = JsonResponse({"status": "error loading map"})
        response.status_code = 400

    return response


# noinspection PyBroadException
@csrf_exempt
def map_download(request):
    """
    map_addr - Map identifier. Field in Map object.

    url: /map_download?path={map_path}

    :param request: GET
    :return:
    {"map": map_obj.map_array}, status code 200
    {"map": "Failed"}, status code 404
    """
    try:
        map_path = str(request.GET.get('path'))
        map_obj = Map.objects.get(path=map_path)
        with open(f"resources/maps/{map_obj.path}.json", "r") as f:
            map_json: dict = json.loads(f.read())
        response = JsonResponse(map_json)
        response.status_code = 200
    except Exception:
        response = JsonResponse({"map": "failed"})
        response.status_code = 404

    return response


def serve_new_instance(request):
    """
    Used for new user, that wants to download game.

    url: /download-full-game

    :param request: GET
    :return:
    Stream containing zipfile with all game files including graphic and music.
    """
    files = Graphic.objects.all()

    filenames = []
    zipfile_name = "zip_z_plikami"

    print("XDDDDDDD")

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

    url: /submit-update

    :param request: GET
    ver = version number : positive integer
    :return:
    {"update": "ok"}, status code 200
    """
    t = Test.objects.get(name="game")
    date = datetime.now()

    _, _, r1, r2, _, m1, m2, _, d1, d2, *_ = str(date)

    build = ""

    build = build + r1 + r2 + m1 + m2 + d1 + d2

    print(build)

    t.actual_build = ver = build

    t.save()

    graphic_files = []
    music_files = []
    other_files = []

    # List all files
    for r, d, f in os.walk(os.getcwd() + os.path.sep + "data"):
        for file in f:
            file_path: str = os.path.join(r, file).replace("\\", "/")
            file_path = "." + re.search(r".+(/data.+)", file_path).group(1)
            if file.endswith(".png"):
                graphic_files.append(file_path)
            elif file.endswith(".ogg"):
                music_files.append(file_path)
            else:
                other_files.append(file_path)

    # Clean undesired files

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

    # Update database

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


def serve_newest_update(request):
    """
    user_identity - unique user id.
    System for updating game files.
    url: /request-update?version={version}
    :param request: GET
    :return:
    """

    version = int(request.GET.get('version', '-1'))

    actual_build = Test.objects.get(name="game").actual_build

    if version >= actual_build:
        response = JsonResponse({"status": "up-to-date"})
        response.status_code = 200
    else:
        files = []

        all_graphic = Graphic.objects.all()
        all_music = Music.objects.all()
        all_other = Other.objects.all()

        zip_name = f"update_{actual_build}.zip"

        [files.append(b.path) for b in all_graphic if b.build > version]
        [files.append(b.path) for b in all_music if b.build > version]
        [files.append(b.path) for b in all_other if b.build > version]

        print(files)

        response = HttpResponse(content_type='application/zip')
        response.set_cookie("build", actual_build)

        print(response.cookies['build'])

        zip_file = zipfile.ZipFile(response, 'w')
        for filename in files:
            zip_file.write(filename)
        response['Content-Disposition'] = f'attachment; filename={zip_name}'

        print(response.items())

    return response


def list_all_maps(request):
    maps = Map.objects.all()

    map_list = []

    for m in maps:
        map_list.append(m.path)

    response = JsonResponse({"maps": map_list})
    response.status_code = 200

    return response
