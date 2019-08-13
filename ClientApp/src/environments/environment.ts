// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: false,
  apiUrl: 'http://Rpi1:5000',
  apiUrlFan: 'http://Fan2',
  apiUrlCam1: 'http://RpiZero1:8080',
  apiUrlCam2: 'http://RpiZero2:8080',
  apiUrlCam3: 'http://RpiZero3:8080',
  apiUrlCam4: 'http://RpiZero4:8085',
  apiUrlCam5: 'http://Rpi1:8080'
  //apiUrl: 'http://192.168.0.184',
  //apiUrlFan: 'http://192.168.0.230',
  //apiUrlCam1: 'http://192.168.0.40',
  //apiUrlCam2: 'http://192.168.0.58'
};
