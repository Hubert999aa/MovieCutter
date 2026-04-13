#!/bin/sh
set -e

cat <<EOF >/usr/share/nginx/html/app-config.js
window.__appConfig = {
  apiUrl: "${API_URL:-http://localhost:8004}"
};
EOF

exec nginx -g "daemon off;"
