# lamp

A lamp simulated device to test DomoticASW

## Docker Hub

[Docker Hub - fracarluccii/lamp](https://hub.docker.com/repository/docker/fracarluccii/lamp/general)

## Run with Docker

To run the lamp device using Docker, you can use the following commands:

```bash
docker pull fracarluccii/lamp:latest
docker run -d -p 8080:80 -e NAME=Lamp-01 fracarluccii/lamp
```

IF you want you can pass the name of the lamp as an environment variable `NAME`

## Endpoints

| Metodo | URL                       | Descrizione                     |
| ------ | ------------------------- | ------------------------------- |
| GET    | `/check-status`           | Current status of the lamp      |
| POST   | `/register`               | Register the lamp in the server |
| POST   | `/execute/turn-on`        | Turn on the lamp                |
| POST   | `/execute/turn-off`       | Turn off the lamp               |
| POST   | `/execute/set-brightness` | Set the brightness of the lamp  |
| POST   | `/execute/set-color`      | Set the color of the lamp       |

Body example for setting brightness:

```json
{
  "input": 50
}
```

Body example for setting color:

```json
{
  "input": "#CAFF70"
}
```