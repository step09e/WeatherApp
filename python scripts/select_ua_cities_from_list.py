import json

with open('city.list.json',"r") as data_file:
    result = list()
    for line in data_file:
        json_obj = json.loads(line)
        country = json_obj.get("country","error")
        if (country.lower()=="ua"):
            result.append(dict({"city":json_obj.get("name","error").encode("utf-8"), "id":json_obj.get("_id","error")}))
with open('ua_cities.json','w') as out:
    i = 0
    dbls_count = 0
    for item in result:
        #===================== проверка на копии, так как в исходном файле, оказывется, есть идентичные названия с разными (!) айдишниками
        unique = True
        j = 0
        for dbl_item in result:
            city1 = item.get("city","none")
            city2 = dbl_item.get("city","none")
            if ( i != j and city1 == city2 ):
                unique = False
                j = j+1
                i = i+1
                break
            j = j+1
        if (unique == False):
            dbls_count = dbls_count+1;
            continue
        #======================
        s = json.dumps(item, ensure_ascii=False) #эту команду пришлось долго искать, так как обычный json.dump не поддерживает уникод
        out.write(s)
        out.write("\n")
        i = i+1
print "task completed. doubles found= " + str(dbls_count);
