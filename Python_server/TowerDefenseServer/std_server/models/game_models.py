from django.db import models


class Map(models.Model):
    path = models.CharField(max_length=200, unique=True)
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
