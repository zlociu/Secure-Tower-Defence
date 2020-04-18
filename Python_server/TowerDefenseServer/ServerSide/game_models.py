from django.db import models
from .setter_models import Player


class Map(models.Model):
    map_address = models.CharField(max_length=127, unique=True)
    player_address = models.ForeignKey(Player, on_delete=models.CASCADE, verbose_name="player_address")
    map_array = models.CharField(max_length=512)
    validationTimeFrom = models.DateField()
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)


class Mob(models.Model):
    name = models.CharField(max_length=128)
    type = models.PositiveIntegerField()

    attack_damage = models.PositiveIntegerField()
    attack_range = models.FloatField()
    fire_rate = models.FloatField()

    health = models.PositiveIntegerField()
    speed = models.FloatField()

    level = models.PositiveIntegerField()
    points_remaining = models.PositiveIntegerField()
    # ?????????????????
    spawn_time = models.PositiveIntegerField()

    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)


class Tower(models.Model):
    name = models.CharField(max_length=128)
    type = models.PositiveIntegerField()

    attack_damage = models.PositiveIntegerField()
    fire_rate = models.FloatField()

    health = models.PositiveIntegerField()

    level = models.PositiveIntegerField()
    points_remaining = models.PositiveIntegerField()
    # ?????????????????
    spawn_time = models.PositiveIntegerField()

    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)

