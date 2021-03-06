#!/usr/bin/env python
import piexif
import array
import utils

en = u"ana"
test =  bytearray(en, "utf-16-le")
exif_dict = piexif.load("C:/Data/test.jpg")

comment = bytearray(exif_dict["0th"][piexif.ImageIFD.XPComment])
keywords = bytearray(exif_dict["0th"][piexif.ImageIFD.XPKeywords])

# print (comment.decode("utf16").rstrip('\x00'))
# print (keywords.decode("utf16").rstrip('\x00'))


new_keywords = bytearray("content=ana.jpg,style=van_gogh.jpg,iterations=500".encode("utf-16-le"))
exif_dict["0th"][piexif.ImageIFD.XPKeywords] = new_keywords

newexif = {"0th": {}, "Exif" : {}, "GPS" : {}, "1st" : {}}
newexif["0th"][piexif.ImageIFD.XPKeywords] = new_keywords
newexif["0th"][piexif.ImageIFD.XPAuthor] = bytearray("tensorflow neural style".encode("utf-16-le"))
newexif["0th"][piexif.ImageIFD.XPTitle] = bytearray("ana.jpg".encode("utf-16-le"))
newexif["0th"][piexif.ImageIFD.XPSubject] = bytearray("van_gogh_die_sterne.jpg".encode("utf-16-le"))
newexif["0th"][piexif.ImageIFD.XPComment] = bytearray("iteration=500,size=700,cw=5,sw=100".encode("utf-16-le"))

exif_bytes = piexif.dump(newexif)
piexif.insert(exif_bytes, "C:/Data/test.jpg")