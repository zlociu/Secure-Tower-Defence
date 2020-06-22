import json
import os
import re
import shutil
import time
import zipfile

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

    game_build = GameBuild.objects.get(name="game").version

    try:
        User.objects.create_user(username=username, password=passwd, version=game_build)
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
        version = GameBuild.objects.get(name="game").version
        response = JsonResponse({"status": "logged", "version": version})
        response.status_code = 201
    else:
        response = JsonResponse({"status": "invalid login"})
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

    for f in files.all():
        filenames.append(f.path)

    print(filenames)

    response = HttpResponse(content_type='application/zip')
    zip_file = zipfile.ZipFile(response, 'w')
    for filename in filenames:
        zip_file.write(filename)
    response['Content-Disposition'] = f'attachment; filename={zipfile_name}'
    return response


# noinspection PyBroadException
def map_current_data(request):
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
        GameBuild.objects.create(version=0)
        game_build = GameBuild.objects.get(name="game")

    version = int(round(time.time()))

    print("Current versions {}".format(version))

    game_build.version = version
    game_build.save()

    graphic_files = []
    sound_files = []
    level_files = []
    turret_files = []
    enemy_files = []

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
                enemy_files.append(file_path)

    def update_resource_files(file_list: list, db_objects):
        for path in file_list:
            resource_name: str = path.split("/")[-1]
            resource_exists = False
            for db_object in db_objects.all():
                if db_object.path == path and db_object.name == resource_name:
                    resource_exists = True
                    break
            if not resource_exists:
                db_object, _ = db_objects.update_or_create(name=resource_name, path=path)
                db_object.version = version
                db_object.save()

    # noinspection PyTypeChecker
    update_resource_files(graphic_files, Graphic.objects)
    # noinspection PyTypeChecker
    update_resource_files(sound_files, Sound.objects)

    def update_json_documents(file_list: list, db_objects):
        for path in file_list:
            with open(path, 'r') as file:
                resource_name: str = json.loads(file.read())['name']
            resource_exists = False
            for db_object in db_objects.all():
                if db_object.path == path and db_object.name != resource_name:
                    resource_exists = True
                    break
            if not resource_exists:
                db_object, _ = db_objects.update_or_create(name=resource_name, path=path)
                db_object.version = version
                db_object.save()

    # noinspection PyTypeChecker
    update_json_documents(level_files, Level.objects)
    # noinspection PyTypeChecker
    update_json_documents(turret_files, Turret.objects)
    # noinspection PyTypeChecker
    update_json_documents(enemy_files, Enemy.objects)

    response = JsonResponse({"status": "ok", "version": version})
    response.status_code = 200

    return response


# noinspection PyBroadException
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
        GameBuild.objects.create(version=0)
        game_build = GameBuild.objects.get(name="game")

    version = int(round(time.time()))

    print("Current versions {}".format(version))

    game_build.version = version
    game_build.save()

    graphic_files = []
    sound_files = []
    level_files = []
    turret_files = []
    enemy_files = []
    all_files = []

    # List all files
    for root, dirs, paths in os.walk(os.getcwd() + os.path.sep + "staging_data"):
        for path in paths:
            file_path: str = os.path.join(root, path).replace("\\", "/")
            file_path = "." + re.search(r".+(/staging_data.+)", file_path).group(1)
            all_files.append(file_path)
            file_path = file_path.replace("staging_data", "data")
            if "sprites" in root.lower():
                graphic_files.append(file_path)
            elif "sounds" in root.lower():
                sound_files.append(file_path)
            elif "levels" in root.lower():
                level_files.append(file_path)
            elif "turrets" in root.lower():
                turret_files.append(file_path)
            elif "enemies" in root.lower():
                enemy_files.append(file_path)

    if len(all_files) == 0:
        response = JsonResponse({"status": "no change", "version": version})
        response.status_code = 200
        return response

    def clean_undesired_files(db_objects, file_list: list):
        for db_object in db_objects.all():
            if db_object.path not in file_list:
                db_objects.get(path=db_object.path).delete()

    # noinspection PyTypeChecker
    clean_undesired_files(Graphic.objects, graphic_files)
    # noinspection PyTypeChecker
    clean_undesired_files(Sound.objects, sound_files)
    # noinspection PyTypeChecker
    clean_undesired_files(Level.objects, level_files)
    # noinspection PyTypeChecker
    clean_undesired_files(Turret.objects, turret_files)
    # noinspection PyTypeChecker
    clean_undesired_files(Enemy.objects, enemy_files)

    def update_resource_files(file_list: list, db_objects):
        for path in file_list:
            resource_name = path.split("/")[-1]
            for db_object in db_objects.all():
                if db_object.path == path and db_object.name != resource_name:
                    db_objects.get(path=path).delete()
            db_object, _ = db_objects.update_or_create(name=resource_name, path=path)
            db_object.version = version
            db_object.save()

    # noinspection PyTypeChecker
    update_resource_files(graphic_files, Graphic.objects)
    # noinspection PyTypeChecker
    update_resource_files(sound_files, Sound.objects)

    def update_json_documents(file_list: list, db_objects):
        for path in file_list:
            with open(path.replace("data", "staging_data"), 'r') as file:
                resource_name: str = json.loads(file.read())['name']
            for db_object in db_objects.all():
                if db_object.path == path and db_object.name != resource_name:
                    db_objects.get(path=path).delete()
            db_object, _ = db_objects.update_or_create(name=resource_name, path=path)
            db_object.version = version
            db_object.save()

    # noinspection PyTypeChecker
    update_json_documents(level_files, Level.objects)
    # noinspection PyTypeChecker
    update_json_documents(turret_files, Turret.objects)
    # noinspection PyTypeChecker
    update_json_documents(enemy_files, Enemy.objects)

    for file_path in all_files:
        data_file_path = file_path.replace("staging_data", "data")
        os.makedirs(os.path.dirname(data_file_path), exist_ok=True)
        shutil.copyfile(file_path, data_file_path)
        os.remove(file_path)

    response = JsonResponse({"status": "ok", "version": version})
    response.status_code = 200

    return response


def serve_update(request):
    """
    user_identity - unique user id.
    System for updating game files.
    url: /request-update?version={version}
    :param request: GET
    :return:
    """

    version = int(request.GET.get('version', '-1'))

    current_version = GameBuild.objects.get(name="game").version

    if version >= current_version:
        response = JsonResponse({"status": "up-to-date"})
        response.status_code = 200
    else:
        files = []

        all_graphic = Graphic.objects.all()
        all_sounds = Sound.objects.all()

        zip_name = f"update_{current_version}.zip"

        [files.append(db_object.path) for db_object in all_graphic if db_object.version > version]
        [files.append(db_object.path) for db_object in all_sounds if db_object.version > version]

        print(files)

        response = HttpResponse(content_type='application/zip')

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

    map_list.sort()
    response = JsonResponse({"maps": map_list})
    response.status_code = 200

    return response
