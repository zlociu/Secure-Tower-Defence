import os
from zipfile import ZipFile

import requests


def send_map(map_array):

    content = {"map": map_array, "player_address": "player_address_1"}

    r = requests.post('http://127.0.0.1:8000/map_upload', data=content)

    print(r.json())


def request_map(map_addr):

    r = requests.get(f'http://127.0.0.1:8000/map_download/{map_addr}')

    print(r.json())


def get_full_new_version():
    url = 'http://127.0.0.1:8000/download_full_game'

    path = "client_files\\files.zip"
    dst_path = "client_files\\files"

    response = requests.get(url, stream=True)
    handle = open(path, "wb")
    for chunk in response.iter_content(chunk_size=512):
        if chunk:
            handle.write(chunk)
    handle.close()

    print(path)

    with ZipFile(path, "r") as zip:
        zip.extractall(dst_path)

    os.remove(path)


def request_update(login):
    url = 'http://127.0.0.1:8000/request_update'

    response = requests.get(f"{url}/{login}", stream=True)

    new_build = response.cookies['build']

    path = f"client_files/update_{new_build}.zip"
    dst_path = "client_files/files"

    handle = open(path, "wb")
    for chunk in response.iter_content(chunk_size=512):
        if chunk:
            handle.write(chunk)
    handle.close()

    print(new_build)

    with ZipFile(path, "r") as zip:
        zip.extractall(dst_path)


def register(login, passwd):
    content = {"username": login, "password": passwd}

    r = requests.post('http://127.0.0.1:8000/register', data=content)


def login(login, passwd):
    content = {"username": login, "password": passwd}

    r = requests.post('http://127.0.0.1:8000/login', data=content)


if __name__ == '__main__':

    # login()
    # map_array = "1; 1; 1; 1; 1; 2; 2; 2; 2; 2; 3; 3; 3; 3; 3; 4; 4; 4; 4; 4; 5; 5; 5; 5; 5"
    # send_map(map_array)
    # map_addr = "random_address"
    # request_map(map_addr)

    register("3", "abcde")
    login("3", "abcde")
    # get_full_new_version()
    request_update("3")
