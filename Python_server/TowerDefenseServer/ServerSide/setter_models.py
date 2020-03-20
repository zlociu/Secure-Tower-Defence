from django.db import models

# Create your models here.


class Test(models.Model):
    name = models.CharField(max_length=127)
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)


class User(models.Model):
    identity = models.CharField()
    login = models.CharField()
    password = models.CharField()
    public_key = models.CharField()
    private_key = models.CharField()
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)


class Player(models.Model):
    identity = models.ForeignKey(User, on_delete=models.CASCADE, verbose_name="user")
    user_address = models.CharField()
    level = models.PositiveIntegerField()
    points = models.PositiveIntegerField()
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)


class KeysRegister(models.Model):
    user_address = models.ForeignKey(Player, on_delete=models.CASCADE, verbose_name="player")
    public_key = models.CharField()
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)
