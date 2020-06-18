from django.db import models


class Graphic(models.Model):
    name = models.CharField(max_length=128, unique=True)
    path = models.CharField(max_length=128, unique=True)
    build = models.PositiveIntegerField()
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)


class Sound(models.Model):
    path = models.CharField(max_length=128, unique=True)
    build = models.PositiveIntegerField()
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)