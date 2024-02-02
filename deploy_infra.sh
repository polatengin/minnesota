PROJECT_PREFIX=$(cat /dev/urandom | tr -dc 'a-z0-9' | fold -w 6 | head -n 1)
LOCATION="westus"

if [ -f .minnesota.json ]; then
  PROJECT_PREFIX=$(jq -r '.project_prefix' .minnesota.json)
  LOCATION=$(jq -r '.location' .minnesota.json)
else
  echo "{\"project_prefix\": \"$PROJECT_PREFIX\", \"location\": \"$LOCATION\"}" > .minnesota.json
fi

az group create --name "${PROJECT_PREFIX}-rg" --location "${LOCATION}"
az search service create --name "${PROJECT_PREFIX}-search" --resource-group "${PROJECT_PREFIX}-rg" --sku "free" --location "${LOCATION}"
