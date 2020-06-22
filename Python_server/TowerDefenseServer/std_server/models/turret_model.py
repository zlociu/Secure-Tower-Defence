from django.db import models


class Turret(models.Model):
    name = models.CharField(max_length=128, unique=True)
    path = models.CharField(max_length=128, unique=True)
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)
    version = models.PositiveIntegerField(default=0)
