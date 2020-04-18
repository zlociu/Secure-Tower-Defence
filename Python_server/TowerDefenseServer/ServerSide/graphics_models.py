from django.db import models


class Graphic(models.Model):
    path = models.CharField(max_length=200, unique=True)
    build = models.PositiveIntegerField()
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)


class Music(models.Model):
    path = models.CharField(max_length=200, unique=True)
    build = models.PositiveIntegerField()
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)


class Other(models.Model):
    path = models.CharField(max_length=200, unique=True)
    build = models.PositiveIntegerField()
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)