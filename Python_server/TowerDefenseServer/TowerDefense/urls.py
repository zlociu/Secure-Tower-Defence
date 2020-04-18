"""TowerDefense URL Configuration

The `urlpatterns` list routes URLs to views. For more information please see:
    https://docs.djangoproject.com/en/3.0/topics/http/urls/
Examples:
Function views
    1. Add an import:  from my_app import views
    2. Add a URL to urlpatterns:  path('', views.home, name='home')
Class-based views
    1. Add an import:  from other_app.views import Home
    2. Add a URL to urlpatterns:  path('', Home.as_view(), name='home')
Including another URLconf
    1. Import the include() function: from django.urls import include, path
    2. Add a URL to urlpatterns:  path('blog/', include('blog.urls'))
"""
from django.contrib import admin
from django.urls import path, include
from ServerSide.views import (
    test_json,
    test_jpg,
    test_db,
    login,
    setup,
    map_upload,
    map_download,
    send_newest_version
)


urlpatterns = [
    path('admin/', admin.site.urls),
    path('test_json', test_json),
    path('test_db', test_db),
    path('test_pics', test_jpg),
    path('login', login),
    path('setup', setup),
    path('map_upload', map_upload),
    path('request_update', send_newest_version),
    path('map_download/<str:map_id>', map_download),

]
