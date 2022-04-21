﻿CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "DeviceCodes" (
    "UserCode" character varying(200) NOT NULL,
    "DeviceCode" character varying(200) NOT NULL,
    "SubjectId" character varying(200) NULL,
    "SessionId" character varying(100) NULL,
    "ClientId" character varying(200) NOT NULL,
    "Description" character varying(200) NULL,
    "CreationTime" timestamp with time zone NOT NULL,
    "Expiration" timestamp with time zone NOT NULL,
    "Data" character varying(50000) NOT NULL,
    CONSTRAINT "PK_DeviceCodes" PRIMARY KEY ("UserCode")
);

CREATE TABLE "Keys" (
    "Id" text NOT NULL,
    "Version" integer NOT NULL,
    "Created" timestamp with time zone NOT NULL,
    "Use" text NULL,
    "Algorithm" character varying(100) NOT NULL,
    "IsX509Certificate" boolean NOT NULL,
    "DataProtected" boolean NOT NULL,
    "Data" text NOT NULL,
    CONSTRAINT "PK_Keys" PRIMARY KEY ("Id")
);

CREATE TABLE "PersistedGrants" (
    "Key" character varying(200) NOT NULL,
    "Type" character varying(50) NOT NULL,
    "SubjectId" character varying(200) NULL,
    "SessionId" character varying(100) NULL,
    "ClientId" character varying(200) NOT NULL,
    "Description" character varying(200) NULL,
    "CreationTime" timestamp with time zone NOT NULL,
    "Expiration" timestamp with time zone NULL,
    "ConsumedTime" timestamp with time zone NULL,
    "Data" character varying(50000) NOT NULL,
    CONSTRAINT "PK_PersistedGrants" PRIMARY KEY ("Key")
);

CREATE UNIQUE INDEX "IX_DeviceCodes_DeviceCode" ON "DeviceCodes" ("DeviceCode");

CREATE INDEX "IX_DeviceCodes_Expiration" ON "DeviceCodes" ("Expiration");

CREATE INDEX "IX_Keys_Use" ON "Keys" ("Use");

CREATE INDEX "IX_PersistedGrants_ConsumedTime" ON "PersistedGrants" ("ConsumedTime");

CREATE INDEX "IX_PersistedGrants_Expiration" ON "PersistedGrants" ("Expiration");

CREATE INDEX "IX_PersistedGrants_SubjectId_ClientId_Type" ON "PersistedGrants" ("SubjectId", "ClientId", "Type");

CREATE INDEX "IX_PersistedGrants_SubjectId_SessionId_Type" ON "PersistedGrants" ("SubjectId", "SessionId", "Type");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20220419182320_Grants', '6.0.4');

COMMIT;

