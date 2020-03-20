from django.shortcuts import render
from django.http import JsonResponse
from django.http import FileResponse
# Create your views here.


def test_json(request):
    response = JsonResponse({"foo": "bar"})
    return response


def test_jpg(request):
    response = FileResponse(open('ServerSide/files/test_file.png', 'rb'))
    return response
