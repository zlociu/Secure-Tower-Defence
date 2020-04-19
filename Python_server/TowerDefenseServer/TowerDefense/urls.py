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
    login,
    setup,
    map_upload,
    map_download,
    serve_newest_update,
    submit_update,
    serve_new_instance
)


urlpatterns = [
    # Admin
    path('admin/', admin.site.urls),

    # User
    path('login', login),
    path('setup', setup),

    # Map
    path('map_upload', map_upload),
    path('map_download/<str:map_id>', map_download),

    # Updates
    path('download_full_game', serve_new_instance),
    path('submit_update', submit_update),
    path('request_update/<str:user_identity>', serve_newest_update),
]
