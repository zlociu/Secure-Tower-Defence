import json
import os
from datetime import datetime
from zipfile import ZipFile

import requests


def send_map(map_array, map_path):
    content = {"map": map_array, "path": map_path}
    r = requests.post('http://127.0.0.1:8000/map-upload', data=json.dumps(content))

    print(r.json())


def request_map(map_path):
    r = requests.get(f'http://127.0.0.1:8000/map-download?path={map_path}')

    print(r.json())


def get_full_new_version():
    url = 'http://127.0.0.1:8000/download-full-game'

    path = "client_files\\files.zip"
    dst_path = "client_files\\files"

    response = requests.get(url, stream=True)

    handle = open(path, "wb")

    for chunk in response.iter_content(chunk_size=512):
        if chunk:
            handle.write(chunk)
    handle.close()

    print(path)

    with ZipFile(path, "r") as zip_file:
        zip_file.extractall(dst_path)

    os.remove(path)


def request_update():
    url = 'http://127.0.0.1:8000/request-update'

    date = datetime.now()

    _, _, r1, r2, _, m1, m2, _, d1, d2, *_ = str(date)

    version = ""

    version = version + r1 + r2 + m1 + m2 + d1 + d2

    response = requests.get(f"{url}?version={version}", stream=True)

    try:
        new_version = response.cookies['version']

        path = f"client_files/update_{new_version}.zip"
        dst_path = "client_files/files"

        handle = open(path, "wb")
        for chunk in response.iter_content(chunk_size=512):
            if chunk:
                handle.write(chunk)
        handle.close()

        print(new_version)

        with ZipFile(path, "r") as zip_file:
            zip_file.extractall(dst_path)
    except KeyError:
        print("Up-to-date")


def register(reg_usr_login, password):
    content = {"username": reg_usr_login, "password": password}

    requests.post('http://127.0.0.1:8000/register', data=content)


def login(lg_usr_login, passwd):
    content = {"username": lg_usr_login, "password": passwd}

    requests.post('http://127.0.0.1:8000/login', data=content)


if __name__ == '__main__':
    map_array1 = [
        {"row": [0, 0, 0, 0]},
        {"row": [1, 1, 0, 0]},
        {"row": [0, 1, 1, 0]},
        {"row": [0, 0, 0, 0]}
    ]
    map_array2 = [
        {"row": [0, 0, 0, 0, 0, 0]},
        {"row": [1, 1, 0, 0, 0, 0]},
        {"row": [0, 1, 0, 0, 0, 0]},
        {"row": [0, 1, 1, 1, 1, 0]},
        {"row": [0, 0, 0, 0, 1, 0]},
        {"row": [0, 1, 1, 1, 1, 0]},
        {"row": [0, 0, 0, 0, 0, 0]}
    ]
    send_map(map_array1, 'map1')
    send_map(map_array2, 'map2')
    request_map('map2')

    register("3", "abcde")
    login("3", "abcde")
    # get_full_new_version()
    request_update()
