import sqlite3

# Nombre o ruta de tu archivo de base de datos SQLite
DB_PATH = "dressly.db"

# Sentencias SQL para agregar columnas si no existen
SQL_MIGRATIONS = [
    "ALTER TABLE Prendas ADD COLUMN EsDonada INTEGER NOT NULL DEFAULT 0;",
    "ALTER TABLE Prendas ADD COLUMN LoteId INTEGER NULL;",
]

def ejecutar_migracion():
    try:
        print(f"Conectando a SQLite ({DB_PATH})...")
        conn = sqlite3.connect(DB_PATH)
        cursor = conn.cursor()

        for sql in SQL_MIGRATIONS:
            try:
                print(f"Ejecutando script de migracion: {sql.strip()}")
                cursor.execute(sql)
                conn.commit()
            except sqlite3.OperationalError as e:
                if "duplicate column name" in str(e).lower():
                    print("Aviso: La columna ya existe en la tabla Prendas.")
                else:
                    print(f"Error operativo en SQLite: {e}")

        print("¡Migracion ejecutada con exito!")

        cursor.close()
        conn.close()

    except Exception as error:
        print(f"Error inesperado: {error}")

if __name__ == "__main__":
    ejecutar_migracion()