# from hashlib import sha256
# from hashlib import sha1
# import json
# import rsa
# # import transaction
# # import player
# from django.db import models
#
#
# class User(models.Model):
#     identity = models.CharField(max_length=160, unique=True, primary_key=True)
#     login = models.CharField(max_length=127, unique=True)
#     password = models.CharField(max_length=256)
#     public_key = models.CharField(max_length=256)
#     created_at = models.DateTimeField(auto_now_add=True)
#     updated_at = models.DateTimeField(auto_now=True)
#
#     def savePublicKey(self):
#         """
#         Saves public key into file named 'identity'.PEM
#         """
#         key = self.publicKey.save_pkcs1()
#         with open(self.identity + '_e.PEM', mode='wb') as publicFile:
#             publicFile.write(key)
#
#     def savePrivateKey(self):
#         """
#         Saves private key into file named 'identity'.PEM
#         """
#         key = self.privateKey.save_pkcs1()
#         with open(self.identity + '_d.PEM', mode='wb') as privateFile:
#             privateFile.write(key)
#
#     def loadPrivateKey(self):
#         """
#         Loads and return private key from file
#         """
#         with open(self.identity + '_d.PEM', mode='rb') as privatefile:
#             keydata = privatefile.read()
#         privKey = self.privateKey.load_pkcs1(keydata)
#         return privKey
#
#     def loadPublicKey(self):
#         """
#         Loads and return public key from file
#         """
#         with open(self.identity + '_e.PEM', mode='rb') as publicfile:
#             keydata = publicfile.read()
#         pubKey = self.publicKey.load_pkcs1(keydata)
#         return pubKey
#
#     @staticmethod
#     def singTransaction(pKey: rsa.PrivateKey, data):
#         """
#         returns signature of transaction (in bytes)
#         """
#         signed = rsa.sign(data, pKey,'SHA-256')
#         return signed
#
#     @staticmethod
#     def verifyTransaction(pKey: rsa.PublicKey, data, signature):
#         """
#         returns if transaction is sign by the owner
#         """
#         try:
#             rsa.verify(data, signature, pKey)
#             return True
#         except:
#             return False
#
#     @staticmethod
#     def hashPassword(passwd):
#         return sha256(passwd.encode()).hexdigest()
#
#     def description(self):
#         txt = " id: {} \n login: {} \n password: {} \n"
#         return txt.format(self.identity, self.login, self.password)
#
#     def saveAsJSON(self):
#         """
#         saves user as JSON file
#         """
#         user_string = json.dumps({
#                                   "login":self.login,
#                                   "password":self.password,
#                                   "identity":self.identity
#                                  }, sort_keys=True)
#         with open(self.identity + '_user.json',mode='wb') as userFile:
#             userFile.write(bytes(user_string.encode()))
#
#     @staticmethod
#     def loadFromJSON(fileName):
#         with open(fileName + '_user.json',mode='rb') as userFile:
#             user_string = userFile.read()
#         u1 = dict(json.loads(user_string))
#         usr = User(u1['login'], u1['password'])
#         usr.identity = u1['identity']
#         usr.privateKey = usr.loadPrivateKey()
#         usr.publicKey = usr.loadPublicKey()
#         return usr
#
#
# #print(sha256(t1.transactionJSON().encode()).hexdigest())
# #isOK = User.verifyTransaction(u1.publicKey, t1.transactionJSON().encode(), sign)
# #print(isOK)
