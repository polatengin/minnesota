PROJECT_PREFIX=$(cat /dev/urandom | tr -dc 'a-z0-9' | fold -w 6 | head -n 1)
LOCATION="westus"

if [ -f .minnesota.json ]; then
  PROJECT_PREFIX=$(jq -r '.project_prefix' .minnesota.json)
  LOCATION=$(jq -r '.location' .minnesota.json)
else
  echo "{\"project_prefix\": \"$PROJECT_PREFIX\", \"location\": \"$LOCATION\"}" > .minnesota.json
fi

az group create --name "${PROJECT_PREFIX}-rg" --location "${LOCATION}"
az search service create --resource-group "${PROJECT_PREFIX}-rg" --name "${PROJECT_PREFIX}-search" --location "${LOCATION}" --sku "basic" --semantic-search "standard"

export SEARCH_ENDPOINT="https://${PROJECT_PREFIX}-search.search.windows.net"
export SEARCH_ADMIN_KEY=$(az search admin-key show --resource-group "${PROJECT_PREFIX}-rg" --service-name "${PROJECT_PREFIX}-search" --query "primaryKey" -o "tsv")
