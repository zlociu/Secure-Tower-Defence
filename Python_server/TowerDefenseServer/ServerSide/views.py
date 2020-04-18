import zipfile
from datetime import datetime

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
from ServerSide.graphics_models import Graphic
from ServerSide.game_models import Map


def setup(request):
    User.objects.create(identity="test_user_1",
                        login="test_user",
                        password="admin",
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
    Graphic.objects.create(path="test")
    response = JsonResponse({"udane": "setup"})
    return response


def test_json(request):
    response = JsonResponse({"foo": "bar"})
    return response


def test_jpg(request):
    response = FileResponse(open('ServerSide/files/test_file.png', 'rb'))
    return response


def test_db(request):
    Mob.objects.create(name="mob", type=1, attack_damage=10, attack_range=10.0, fire_rate=2.0, health=100, speed=23.3,
                       level=9, points_remaining=24, spawn_time=22)
    Test.objects.create(name="test")
    Graphic.objects.create(path="test")
    response = FileResponse(open('ServerSide/files/test_file.png', 'rb'))
    return response


@csrf_exempt
def login(request):
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

        response = JsonResponse({"status": "mapa załadowana"})
        response.status_code = 200
    except:
        response = JsonResponse({"status": "błąd"})
        response.status_code = 404

    return response


@csrf_exempt
def map_download(request, map_id):
    print(map_id)
    map = Map.objects.get(map_address=map_id)

    response = JsonResponse({"map": map.map_array})
    response.status_code = 200

    return response


def send_newest_version(request):
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
    response['Content-Disposition'] = 'attachment; filename={}'.format(zipfile_name)
    return response

    # file_location = "TowerDefense/Packages/graphics/sprite_1.png"
    #
    # try:
    #     with open(file_location, 'rb') as f:
    #        file_data = f.read()
    #
    #     # sending response
    #     response = HttpResponse(file_data)
    #
    # except IOError:
    #     # handle file not exist case here
    #     response = HttpResponseNotFound('<h1>File not exist</h1>')

    # return response
