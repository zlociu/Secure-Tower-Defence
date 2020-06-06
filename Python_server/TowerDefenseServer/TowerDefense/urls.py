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
from django.urls import path

from ServerSide.views import (
    login_user,
    setup,
    map_upload,
    map_download,
    serve_newest_update,
    submit_update,
    serve_new_instance,
    register,
    list_all_maps
)

urlpatterns = [
    # Admin
    path('admin/', admin.site.urls),
    path('register', register),
    path('login', login_user),

    # User
    path('setup', setup),

    # Map
    path('map-upload', map_upload),
    path('map-download', map_download),
    path('list-maps', list_all_maps),

    # Updates
    path('download-full-game', serve_new_instance),
    path('submit-update', submit_update),
    path('request-update', serve_newest_update),
]
