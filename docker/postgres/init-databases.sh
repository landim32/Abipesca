#!/bin/bash
set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE USER nauth_user WITH PASSWORD '${NAUTH_DB_PASSWORD}';
    CREATE DATABASE nauth_db OWNER nauth_user;
    GRANT ALL PRIVILEGES ON DATABASE nauth_db TO nauth_user;

    CREATE USER nnews_user WITH PASSWORD '${NNEWS_DB_PASSWORD}';
    CREATE DATABASE nnews_db OWNER nnews_user;
    GRANT ALL PRIVILEGES ON DATABASE nnews_db TO nnews_user;

    CREATE USER bazzuca_user WITH PASSWORD '${BAZZUCA_DB_PASSWORD}';
    CREATE DATABASE bazzuca_db OWNER bazzuca_user;
    GRANT ALL PRIVILEGES ON DATABASE bazzuca_db TO bazzuca_user;
EOSQL
