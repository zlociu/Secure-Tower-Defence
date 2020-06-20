import json
import os
import re
import zipfile
from datetime import datetime

import django
from django.contrib.auth import authenticate, logout, login
from django.db.utils import IntegrityError
from django.http import JsonResponse, HttpResponse
from django.views.decorators.csrf import csrf_exempt

from std_server.models.enemy_model import Enemy
from std_server.models.game_build_model import GameBuild
from std_server.models.graphics_model import Graphic
from std_server.models.level_model import Level
from std_server.models.sounds_model import Sound
from std_server.models.turret_model import Turret
from std_server.models.user_model import User


@csrf_exempt
def register(request):
    username = request.POST['username']
    passwd = request.POST['password']

    print(username)
    print(passwd)

    game_build = GameBuild.objects.get(name="game").build

    try:
        User.objects.create_user(username=username, password=passwd, game_build=game_build)
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
@csrf_exempt
def level_download(request):
    """
    url: /level-download?name={map_name}

    :param request: GET
    :return:
    {"map": map_obj.map_array}, status code 200
    {"map": "Failed"}, status code 404
    """
    try:
        map_name = str(request.GET.get('name'))
        map_obj = Level.objects.get(name=map_name)
        with open(map_obj.path, "r") as f:
            map_json: dict = json.loads(f.read())
        response = JsonResponse(map_json)
        response.status_code = 200
    except Exception:
        response = JsonResponse({})
        response.status_code = 404

    return response


# noinspection PyBroadException
@csrf_exempt
def turret_download(request):
    """
    url: /turret-download?name={turret_name}

    :param request: GET
    :return:
    {"turret": map_obj.map_array}, status code 200
    {"map": "Failed"}, status code 404
    """
    try:
        turret_name = str(request.GET.get('name'))
        turret_obj = Turret.objects.get(name=turret_name)
        with open(turret_obj.path, "r") as f:
            turret_json: dict = json.loads(f.read())
        turret_json["baseTexture"] = "/Buildings/" + turret_json["baseTexture"]
        turret_json["weaponTexture"] = "/Buildings/" + turret_json["weaponTexture"]
        turret_json["projectileTexture"] = "/Effects/" + turret_json["projectileTexture"]
        turret_json["uiTexture"] = "/Ui/" + turret_json["uiTexture"]
        turret_json["shootSound"] = re.search(r".+/Sounds(.+)",
                                              Sound.objects.get(name=turret_json['shootSound']).path).group(1)
        response = JsonResponse(turret_json)
        response.status_code = 200
    except Exception:
        response = JsonResponse({})
        response.status_code = 404

    return response


# noinspection PyBroadException
@csrf_exempt
def enemy_download(request):
    """
    url: /enemy-download?name={enemy_name}

    :param request: GET
    :return:
    {"enemy": map_obj.map_array}, status code 200
    {"map": "Failed"}, status code 404
    """
    try:
        enemy_name = str(request.GET.get('name'))
        enemy_obj = Enemy.objects.get(name=enemy_name)
        with open(enemy_obj.path, "r") as f:
            enemy_json: dict = json.loads(f.read())
        enemy_json["texture"] = "/Enemies/" + enemy_json['texture']
        enemy_json["sound"] = re.search(r".+/Sounds(.+)", Sound.objects.get(name=enemy_json['sound']).path).group(1)
        response = JsonResponse(enemy_json)
        response.status_code = 200
    except Exception:
        response = JsonResponse({})
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
    try:
        game_build = GameBuild.objects.get(name="game")
    except Exception:
        GameBuild.objects.create(build=0)
        game_build = GameBuild.objects.get(name="game")

    date = datetime.now()

    _, _, r1, r2, _, m1, m2, _, d1, d2, *_ = str(date)

    build = ""

    build = build + r1 + r2 + m1 + m2 + d1 + d2

    print(build)

    game_build.build = ver = build

    game_build.save()

    graphic_files = []
    sound_files = []
    level_files = []
    turret_files = []
    enemies_files = []

    # List all files
    for root, dirs, paths in os.walk(os.getcwd() + os.path.sep + "data"):
        for path in paths:
            file_path: str = os.path.join(root, path).replace("\\", "/")
            file_path = "." + re.search(r".+(/data.+)", file_path).group(1)
            if "sprites" in root.lower():
                graphic_files.append(file_path)
            elif "sounds" in root.lower():
                sound_files.append(file_path)
            elif "levels" in root.lower():
                level_files.append(file_path)
            elif "turrets" in root.lower():
                turret_files.append(file_path)
            elif "enemies" in root.lower():
                enemies_files.append(file_path)

    # Clean undesired files

    for graphic in Graphic.objects.all():
        if graphic.path not in graphic_files:
            Graphic.objects.get(path=graphic.path).delete()

    for sound in Sound.objects.all():
        if sound.path not in sound_files:
            Sound.objects.get(path=sound.path).delete()

    for level in Level.objects.all():
        if level.path not in level_files:
            Sound.objects.get(path=level.path).delete()

    # Update database

    for path in graphic_files:
        graphic_name = path.split("/")[-1].split(".")[0]
        graphic, _ = Graphic.objects.update_or_create(name=graphic_name, path=path, build=ver)
        graphic.version = ver

    for path in sound_files:
        sound_name = path.split("/")[-1]
        sound, _ = Sound.objects.update_or_create(name=sound_name, path=path, build=ver)
        sound.version = ver

    for path in level_files:
        with open(path, 'r') as file:
            level_name: str = json.loads(file.read())['name']
        level_obj, _ = Level.objects.update_or_create(name=level_name, path=path)
        level_obj.version = ver

    for path in turret_files:
        with open(path, 'r') as file:
            turret_name: str = json.loads(file.read())['name']
        turret_obj, _ = Turret.objects.update_or_create(name=turret_name, path=path)
        turret_obj.version = ver

    for path in enemies_files:
        with open(path, 'r') as file:
            enemy_name: str = json.loads(file.read())['name']
        enemy_obj, _ = Enemy.objects.update_or_create(name=enemy_name, path=path)
        enemy_obj.version = ver

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

    build = GameBuild.objects.get(name="game").build

    if version >= build:
        response = JsonResponse({"status": "up-to-date"})
        response.status_code = 200
    else:
        files = []

        all_graphic = Graphic.objects.all()
        all_music = Sound.objects.all()

        zip_name = f"update_{build}.zip"

        [files.append(b.path) for b in all_graphic if b.build > version]
        [files.append(b.path) for b in all_music if b.build > version]

        print(files)

        response = HttpResponse(content_type='application/zip')
        response.set_cookie("build", build)

        print(response.cookies['build'])

        zip_file = zipfile.ZipFile(response, 'w')
        for filename in files:
            zip_file.write(filename)
        response['Content-Disposition'] = f'attachment; filename={zip_name}'

        print(response.items())

    return response


def list_all_maps(request):
    maps = Level.objects.all()

    map_list = []

    for m in maps:
        map_list.append(m.name)

    response = JsonResponse({"maps": map_list})
    response.status_code = 200

    return response
