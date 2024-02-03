# minnesota

Sample project to demonstrate how to use **Azure Search Service** with **.NET Core**.

Using wildcards in **Azure Search** queries is a common requirement. But it's not as _straightforward_ as it seems.

I created this project to demonstrate a few options to use wildcards in **Azure Search** queries.

> _Spoiler alert: None of them works as expected 😢_

## First: Deploying Azure Search Service

After loggin in to **Azure CLI**, run the following command to create a new **Azure Search Service**.

```bash
cd src
source ../deploy_infra.sh
```

The [deploy_infra.sh](./deploy_infra.sh) script will create a new **Resource Group** and an **Azure Search Service** instance. It will also set the `SEARCH_ENDPOINT` and `SEARCH_ADMIN_KEY` environment variables.

> ```bash
> PROJECT_PREFIX=$(cat /dev/urandom | tr -dc 'a-z0-9' | fold -w 6 | head -n 1)
> LOCATION="westus"
> 
> if [ -f .minnesota.json ]; then
>   PROJECT_PREFIX=$(jq -r '.project_prefix' .minnesota.json)
>   LOCATION=$(jq -r '.location' .minnesota.json)
> else
>   echo "{\"project_prefix\": \"$PROJECT_PREFIX\", \"location\": \"$LOCATION\"}" > .minnesota.json
> fi
> 
> az group create --name "${PROJECT_PREFIX}-rg" --location "${LOCATION}"
> az search service create --resource-group "${PROJECT_PREFIX}-rg" --name "${PROJECT_PREFIX}-search" --location "${LOCATION}" --sku "basic" --semantic-search "standard"
> 
> export SEARCH_ENDPOINT="https://${PROJECT_PREFIX}-search.search.windows.net"
> export SEARCH_ADMIN_KEY=$(az search admin-key show --resource-group "${PROJECT_PREFIX}-rg" --service-name "${PROJECT_PREFIX}-search" --query "primaryKey" -o "tsv")
> ```

## Second: Populating the Index

After creating the **Azure Search Service**, you can run the following command to populate the index with some sample data ([./src/data.json](./src/data.json)).

```bash
cd src
dotnet run
```

The [Program.cs](./src/Program.cs) file contains the code to create the index and populate it with some sample data ([./src/data.json](./src/data.json)).

![image](https://github.com/polatengin/minnesota/assets/118744/c4b9817a-60c5-42c0-b6c1-a4bb61f9a523)

After **Search Index** is created and populated with _sample data+, [Program.cs](./src/Program.cs) is going in a _loop_ to ask for a _search query_, run the _query_ and display the _results_.

> According to the [Azure Search Query Simple Syntax](https://learn.microsoft.com/en-us/azure/search/query-simple-syntax), _wildcard_ options are available for **Azure Search** queries.

[Program.cs#L74](./src/Program.cs#L74) adds _wildcard_ character `*` to the end of the _search query_ and runs the _query_.

## Third: Running the Queries

First query is `medical*` and it returns `0` results.

![image](https://github.com/polatengin/minnesota/assets/118744/5f2606af-c20c-4cea-bccb-301e29f1dd29)

---

Second query is `medi*` and it returns `11` results.

![image](https://github.com/polatengin/minnesota/assets/118744/ed4b30fa-590b-41a7-b4e4-6082770120be)

---

Third query is `medic*` and it returns `11` results.

![image](https://github.com/polatengin/minnesota/assets/118744/4ba6e6a9-5e36-4ae8-ab93-43aca700395e)

---

Fourth query is `medica*` and it returns `0` results.

![image](https://github.com/polatengin/minnesota/assets/118744/d2019e2a-5bf6-486c-860b-a9f52989945b)

---

Even if the same queries run on the **Azure Portal**, the results are the same.

---

Running the `medical` query on the **Azure Portal** returns `10` results.

![image](https://github.com/polatengin/minnesota/assets/118744/602b50b8-d432-4bc7-9320-c358c49a56ea)

---

Running the `medi` query on the **Azure Portal** returns `0` results.

![image](https://github.com/polatengin/minnesota/assets/118744/2de8e55a-9e50-4b09-8929-4086a91191e5)

---

Running the `medi*` query on the **Azure Portal** returns `10` results.

![image](https://github.com/polatengin/minnesota/assets/118744/611e10fd-3da1-4d89-ac52-328754ebe32d)

---

Running the `medical*` query on the **Azure Portal** returns `0` results.

![image](https://github.com/polatengin/minnesota/assets/118744/ec355970-fe8e-4af7-a66b-c516085d0790)
