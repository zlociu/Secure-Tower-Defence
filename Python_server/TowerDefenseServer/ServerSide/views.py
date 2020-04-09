from django.shortcuts import render
from django.http import JsonResponse
from django.http import FileResponse
# Create your views here.
from ServerSide.game_models import Mob
from ServerSide.setter_models import Test
from ServerSide.graphics_models import Graphic

def test_json(request):
    response = JsonResponse({"foo": "bar"})
    return response


def test_jpg(request):
    response = FileResponse(open('ServerSide/files/test_file.png', 'rb'))
    return response


def test_db(request):
    Mob.objects.create(name="mob", type=1, attack_damage=10, attack_range=10.0, fire_rate=2.0, health=100, speed=23.3, level=9, points_remaining=24, spawn_time=22)
    Test.objects.create(name="test")
    Graphic.objects.create(path="test")
    response = FileResponse(open('ServerSide/files/test_file.png', 'rb'))
    return response