
CREATE TABLE Client_Machine
(
  id_client_machine int          NOT NULL IDENTITY(1,1),
  id_client         int          NOT NULL,
  Name              varchar(255) NOT NULL,
  CONSTRAINT PK_Client_Machine PRIMARY KEY (id_client_machine)
)

ALTER TABLE Client_Machine
  ADD CONSTRAINT UQ_id_client_machine UNIQUE (id_client_machine)

CREATE TABLE Client_Machine_Disque
(
  id_client_machine_disque int          NOT NULL IDENTITY(1,1),
  id_client_machine        int          NOT NULL,
  Name                     varchar(255) NOT NULL,
  Root_Directory           varchar(255) NOT NULL,
  Label                    varchar(255),
  Type                     varchar(255) NOT NULL,
  Format                   varchar(255) NOT NULL,
  Size                     bigint       NOT NULL,
  Free_Space               bigint       NOT NULL,
  Total_Space              bigint       NOT NULL,
  Free_Space_Percentage    int          NOT NULL,
  Is_System_Drive          bit          NOT NULL,
  CONSTRAINT PK_Client_Machine_Disque PRIMARY KEY (id_client_machine_disque)
)

ALTER TABLE Client_Machine_Disque
  ADD CONSTRAINT UQ_id_client_machine_disque UNIQUE (id_client_machine_disque)

CREATE TABLE Client_Machine_Disque_Application
(
  id_client_machine_disque_app int          NOT NULL IDENTITY(1,1),
  Name                         varchar(255) NOT NULL,
  CONSTRAINT PK_Client_Machine_Disque_Application PRIMARY KEY (id_client_machine_disque_app)
)

ALTER TABLE Client_Machine_Disque_Application
  ADD CONSTRAINT UQ_id_client_machine_disque_app UNIQUE (id_client_machine_disque_app)

CREATE TABLE Client_Machine_Disque_Os
(
  id_client_machine_disque_os int          NOT NULL IDENTITY(1,1),
  id_client_machine_disque    int          NOT NULL,
  Directory                   varchar(255) NOT NULL,
  Architecture                varchar(255) NOT NULL,
  Version                     varchar(255) NOT NULL,
  Product_Name                varchar(255),
  Release_Id                  varchar(255),
  Current_Build               varchar(255),
  Ubr                         varchar(255),
  CONSTRAINT PK_Client_Machine_Disque_Os PRIMARY KEY (id_client_machine_disque_os)
)

ALTER TABLE Client_Machine_Disque_Os
  ADD CONSTRAINT UQ_id_client_machine_disque_os UNIQUE (id_client_machine_disque_os)

CREATE TABLE Relation_Disque_Application
(
  id_client_machine_disque     int          NOT NULL,
  id_client_machine_disque_app int          NOT NULL,
  Product_Name                 varchar(255),
  Path                         varchar(255) NOT NULL,
  Description                  varchar(255),
  File_Version                 varchar(255),
  Product_Version              varchar(255),
  Type                         varchar(255)
)

ALTER TABLE Client_Machine
  ADD CONSTRAINT FK_customersTableName_TO_Client_Machine
    FOREIGN KEY (id_client)
    REFERENCES customersTableName (id_client)

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
