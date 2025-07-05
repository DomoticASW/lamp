# lamp

A lamp simulated device to test DomoticASW

## Docker Hub

[Docker Hub - fracarluccii/domoticasw-lamp](https://hub.docker.com/repository/docker/fracarluccii/domoticasw-lamp/general)

## Run with Docker

To run the lamp device using Docker, you can use the following commands:

```bash
docker run fracarluccii/domoticasw-lamp 
```

### Variables

The following configurations can be passed to the container as environment variables

| Variable name     | Default value   | Explanation                         |
| ----------------- | --------------- | ----------------------------------- |
| NAME              | Lamp-01         | lamp name                           |
| DEVICE_PORT       | 8093            | Port used by the lamp device        |
| SERVER_ADDRESS    | localhost       | Address of the server               |
| SERVER_PORT       | 3000            | Port of the server                  |
| DISCOVERY_ADDRESS | 255.255.255.255 | Address for discovery broadcasts    |
| DISCOVERY_PORT    | 30000           | Port for discovery broadcasts       |

## How to use

At first send <code><\<device-address\>>/register</code> request to the device to register it in the server.

## Properties

- <b>state</b>: The current state of the lamp (on/off).
- <b>brightness</b>: The current brightness level of the lamp (0-100).
- <b>color</b>: The current color of the lamp in RGB format.

## Actions

- <code><\<device-address\>>/execute/turn-on</code>: Turn on the lamp.
- <code><\<device-address\>>/execute/turn-off</code>: Turn off the lamp.
- <code><\<device-address\>>/execute/set-brightness</code>: Set the desired brightness level on the lamp.
- <code><\<device-address\>>/execute/set-color</code>: Set the desired color on the lamp.

Body example for set-brightness:

```json
{
  "input": 50
}
```

Body example for set-color:

```json
{
  "input":
        {
          "r" : 127,
          "g" : 127,
          "b" : 127
        }
}
```

## Events

- <b>turned-on</b>: Triggered when the lamp is turned on.
- <b>turned-off</b>: Triggered when the lamp is turned off.
- <b>brightness-changed</b>: Triggered when the brightness level changes.
- <b>color-changed</b>: Triggered when the color changes.
