from django.db import models


class Sound(models.Model):
    path = models.CharField(max_length=128, unique=True)
    name = models.CharField(max_length=128, unique=True)
    build = models.PositiveIntegerField()
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)
