{{/*
Expand the name of the chart.
*/}}
{{- define "str-keycloak.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "str-keycloak.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "str-keycloak.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "str-keycloak.labels" -}}
helm.sh/chart: {{ include "str-keycloak.chart" . }}
{{ include "str-keycloak.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "str-keycloak.selectorLabels" -}}
app.kubernetes.io/name: {{ include "str-keycloak.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{/*
Create the name of the service account to use
*/}}
{{- define "str-keycloak.serviceAccountName" -}}
{{- if .Values.serviceAccount.create }}
{{- default (include "str-keycloak.fullname" .) .Values.serviceAccount.name }}
{{- else }}
{{- default "default" .Values.serviceAccount.name }}
{{- end }}
{{- end }}

{{/*
Create encrypted database config params
*/}}
{{- define "dbConnectionStringEnc" }}
{{- printf "Host=%s-psql;Port=5432;Database=%s;Username=%s;Password=%s;Tcp Keepalive=true;Timeout=300;CommandTimeout=300;" .Values.namePrefix .Values.sqlDb.name .Values.sqlDb.user .Values.sqlDb.password | b64enc }}
{{- end }}
{{- define "dbNameEnc" }}
{{- printf "%s" .Values.sqlDb.name | b64enc }}
{{- end }}
{{- define "dbUserEnc" }}
{{- printf "%s" .Values.sqlDb.user | b64enc }}
{{- end }}
{{- define "dbPasswordEnc" }}
{{- printf "%s" .Values.sqlDb.password | b64enc }}
{{- end }}
