
CREATE TABLE Client
(
  id_client  int          NOT NULL IDENTITY(1,1),
  Nom_Client varchar(255) NOT NULL,
  CONSTRAINT PK_Client PRIMARY KEY (id_client)
)

ALTER TABLE Client
  ADD CONSTRAINT UQ_id_client UNIQUE (id_client)

CREATE TABLE Client_Machine
(
  id_client_machine int          NOT NULL IDENTITY(1,1),
  id_client         int          NOT NULL,
  Nom_Machine       varchar(255) NOT NULL,
  Sys_Date_Creation datetime     NOT NULL,
  CONSTRAINT PK_Client_Machine PRIMARY KEY (id_client_machine)
)

ALTER TABLE Client_Machine
  ADD CONSTRAINT UQ_id_client_machine UNIQUE (id_client_machine)

CREATE TABLE Client_Machine_Disque
(
  id_client_machine_disque   int          NOT NULL IDENTITY(1,1),
  id_client_machine          int          NOT NULL,
  Nom_Disque                 varchar(255) NOT NULL,
  Dossier_Racine             varchar(255) NOT NULL,
  Label                      varchar(255),
  Type                       varchar(255) NOT NULL,
  Format                     varchar(255) NOT NULL,
  Taille                     bigint       NOT NULL,
  Espace_Disponible          bigint       NOT NULL,
  Espace_Total               bigint       NOT NULL,
  Espace_Disponible_Pourcent int          NOT NULL,
  Is_System_Drive            bit          NOT NULL,
  Sys_Date_Creation          datetime     NOT NULL,
  CONSTRAINT PK_Client_Machine_Disque PRIMARY KEY (id_client_machine_disque)
)

ALTER TABLE Client_Machine_Disque
  ADD CONSTRAINT UQ_id_client_machine_disque UNIQUE (id_client_machine_disque)

CREATE TABLE Client_Machine_Disque_Application
(
  id_client_machine_disque_app int          NOT NULL IDENTITY(1,1),
  Nom_Application              varchar(255) NOT NULL,
  CONSTRAINT PK_Client_Machine_Disque_Application PRIMARY KEY (id_client_machine_disque_app)
)

ALTER TABLE Client_Machine_Disque_Application
  ADD CONSTRAINT UQ_id_client_machine_disque_app UNIQUE (id_client_machine_disque_app)

CREATE TABLE Client_Machine_Disque_Historique
(
  id_client_machine_disque_historique int          NOT NULL IDENTITY(1,1),
  id_client_machine                   int          NOT NULL,
  Nom_Disque                          varchar(255) NOT NULL,
  Dossier_Racine                      varchar(255) NOT NULL,
  Label                               varchar(255),
  Type                                varchar(255) NOT NULL,
  Format                              varchar(255) NOT NULL,
  Taille                              bigint       NOT NULL,
  Espace_Disponible                   bigint       NOT NULL,
  Espace_Total                        bigint       NOT NULL,
  Espace_Disponible_Pourcent          int          NOT NULL,
  Is_System_Drive                     bit          NOT NULL,
  Sys_Date_Creation                   datetime     NOT NULL,
  CONSTRAINT PK_Client_Machine_Disque_Historique PRIMARY KEY (id_client_machine_disque_historique)
)

ALTER TABLE Client_Machine_Disque_Historique
  ADD CONSTRAINT UQ_id_client_machine_disque_historique UNIQUE (id_client_machine_disque_historique)

CREATE TABLE Client_Machine_Disque_Os
(
  id_client_machine_disque_os int          NOT NULL IDENTITY(1,1),
  id_client_machine_disque    int          NOT NULL,
  Dossier                     varchar(255) NOT NULL,
  Architecture                varchar(255) NOT NULL,
  Version                     varchar(255) NOT NULL,
  Nom_Produit                 varchar(255),
  Id_Release                  varchar(255),
  Build_Actuel                varchar(255),
  Update_Build_Revision       varchar(255),
  Sys_Date_Creation           datetime     NOT NULL,
  CONSTRAINT PK_Client_Machine_Disque_Os PRIMARY KEY (id_client_machine_disque_os)
)

ALTER TABLE Client_Machine_Disque_Os
  ADD CONSTRAINT UQ_id_client_machine_disque_os UNIQUE (id_client_machine_disque_os)

CREATE TABLE Client_Machine_Disque_Os_Historique
(
  id_client_machine_disque_os_historique int          NOT NULL IDENTITY(1,1),
  id_client_machine_disque_historique    int          NOT NULL,
  Dossier                                varchar(255) NOT NULL,
  Architecture                           varchar(255) NOT NULL,
  Version                                varchar(255) NOT NULL,
  Nom_Produit                            varchar(255),
  Id_Release                             varchar(255),
  Build_Actuel                           varchar(255),
  Update_Build_Revision                  varchar(255),
  Sys_Date_Creation                      datetime     NOT NULL,
  CONSTRAINT PK_Client_Machine_Disque_Os_Historique PRIMARY KEY (id_client_machine_disque_os_historique)
)

ALTER TABLE Client_Machine_Disque_Os_Historique
  ADD CONSTRAINT UQ_id_client_machine_disque_os_historique UNIQUE (id_client_machine_disque_os_historique)

CREATE TABLE Relation_Disque_Application
(
  id_client_machine_disque     int           NOT NULL,
  id_client_machine_disque_app int           NOT NULL,
  Commentaires                 NVARCHAR(255),
  Nom_Entreprise               NVARCHAR(255),
  Partie_Build_Fichier         INT           NOT NULL,
  Description_Fichier          NVARCHAR(255),
  Partie_Majeure_Fichier       INT           NOT NULL,
  Partie_Mineure_Fichier       INT           NOT NULL,
  Nom_Fichier                  NVARCHAR(255),
  Partie_Privee_Fichier        INT           NOT NULL,
  Version_Fichier              NVARCHAR(50) ,
  Nom_Interne                  NVARCHAR(255),
  Is_Debug                     BIT           NOT NULL,
  Is_Patched                   BIT           NOT NULL,
  Is_Pre_Release               BIT           NOT NULL,
  Is_Private_Build             BIT           NOT NULL,
  Is_Special_Build             BIT           NOT NULL,
  Langage                      NVARCHAR(50) ,
  Copyright                    NVARCHAR(255),
  Trademarks                   NVARCHAR(255),
  Nom_Fichier_Original         NVARCHAR(255),
  Build_Prive                  NVARCHAR(255),
  Partie_Build_Produit         INT           NOT NULL,
  Partie_Majeure_Produit       INT           NOT NULL,
  Partie_Mineure_Produit       INT           NOT NULL,
  Nom_Produit                  NVARCHAR(255),
  Partie_Privee_Produit        INT           NOT NULL,
  Version_Produit              NVARCHAR(50) ,
  Build_Special                NVARCHAR(255),
  Sys_Date_Creation            datetime      NOT NULL,
  CONSTRAINT PK_Relation_Disque_Application PRIMARY KEY (id_client_machine_disque, id_client_machine_disque_app)
)

CREATE TABLE Relation_Disque_Application_Historique
(
  id_client_machine_disque_historique int           NOT NULL,
  id_client_machine_disque_app        int           NOT NULL,
  Commentaires                        NVARCHAR(255),
  Nom_Entreprise                      NVARCHAR(255),
  Partie_Build_Fichier                INT           NOT NULL,
  Description_Fichier                 NVARCHAR(255),
  Partie_Majeure_Fichier              INT           NOT NULL,
  Partie_Mineure_Fichier              INT           NOT NULL,
  Nom_Fichier                         NVARCHAR(255),
  Partie_Privee_Fichier               INT           NOT NULL,
  Version_Fichier                     NVARCHAR(50) ,
  Nom_Interne                         NVARCHAR(255),
  Is_Debug                            BIT           NOT NULL,
  Is_Patched                          BIT           NOT NULL,
  Is_Pre_Release                      BIT           NOT NULL,
  Is_Private_Build                    BIT           NOT NULL,
  Is_Special_Build                    BIT           NOT NULL,
  Langage                             NVARCHAR(50) ,
  Copyright                           NVARCHAR(255),
  Trademarks                          NVARCHAR(255),
  Nom_Fichier_Original                NVARCHAR(255),
  Build_Prive                         NVARCHAR(255),
  Partie_Build_Produit                INT           NOT NULL,
  Partie_Majeure_Produit              INT           NOT NULL,
  Partie_Mineure_Produit              INT           NOT NULL,
  Nom_Produit                         NVARCHAR(255),
  Partie_Privee_Produit               INT           NOT NULL,
  Version_Produit                     NVARCHAR(50) ,
  Build_Special                       NVARCHAR(255),
  Sys_Date_Creation                   datetime      NOT NULL,
  CONSTRAINT PK_Relation_Disque_Application_Historique PRIMARY KEY (id_client_machine_disque_historique, id_client_machine_disque_app)
)

ALTER TABLE Client_Machine
  ADD CONSTRAINT FK_Client_TO_Client_Machine
    FOREIGN KEY (id_client)
    REFERENCES Client (id_client)

ALTER TABLE Client_Machine_Disque
  ADD CONSTRAINT FK_Client_Machine_TO_Client_Machine_Disque
    FOREIGN KEY (id_client_machine)
    REFERENCES Client_Machine (id_client_machine)

ALTER TABLE Client_Machine_Disque_Os
  ADD CONSTRAINT FK_Client_Machine_Disque_TO_Client_Machine_Disque_Os
    FOREIGN KEY (id_client_machine_disque)
    REFERENCES Client_Machine_Disque (id_client_machine_disque)

ALTER TABLE Relation_Disque_Application
  ADD CONSTRAINT FK_Client_Machine_Disque_TO_Relation_Disque_Application
    FOREIGN KEY (id_client_machine_disque)
    REFERENCES Client_Machine_Disque (id_client_machine_disque)

ALTER TABLE Relation_Disque_Application
  ADD CONSTRAINT FK_Client_Machine_Disque_Application_TO_Relation_Disque_Application
    FOREIGN KEY (id_client_machine_disque_app)
    REFERENCES Client_Machine_Disque_Application (id_client_machine_disque_app)

ALTER TABLE Client_Machine_Disque_Os_Historique
  ADD CONSTRAINT FK_Client_Machine_Disque_Historique_TO_Client_Machine_Disque_Os_Historique
    FOREIGN KEY (id_client_machine_disque_historique)
    REFERENCES Client_Machine_Disque_Historique (id_client_machine_disque_historique)

ALTER TABLE Relation_Disque_Application_Historique
  ADD CONSTRAINT FK_Client_Machine_Disque_Historique_TO_Relation_Disque_Application_Historique
    FOREIGN KEY (id_client_machine_disque_historique)
    REFERENCES Client_Machine_Disque_Historique (id_client_machine_disque_historique)

ALTER TABLE Relation_Disque_Application_Historique
  ADD CONSTRAINT FK_Client_Machine_Disque_Application_TO_Relation_Disque_Application_Historique
    FOREIGN KEY (id_client_machine_disque_app)
    REFERENCES Client_Machine_Disque_Application (id_client_machine_disque_app)

ALTER TABLE Client_Machine_Disque_Historique
  ADD CONSTRAINT FK_Client_Machine_TO_Client_Machine_Disque_Historique
    FOREIGN KEY (id_client_machine)
    REFERENCES Client_Machine (id_client_machine)
