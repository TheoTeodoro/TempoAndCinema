import sqlite3

conn = sqlite3.connect("catalog.db")
cursor = conn.cursor()

with open("database.sql", "r", encoding="utf-8") as f:
    sql_script = f.read()

cursor.executescript(sql_script)
conn.commit()
conn.close()

print("catalog.db criado com sucesso.")
