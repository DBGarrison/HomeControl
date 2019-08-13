import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { trigger, state, style, animate, transition } from '@angular/animations';

@Component({
  selector: 'pi-camera7',
  templateUrl: './picam7.component.html',
  animations: [
    // Each unique animation requires its own trigger. The first argument of the trigger function is the name
    trigger('rotatedState', [
      state('default', style({ transform: 'rotate(0)' })),
      state('rotated', style({ transform: 'rotate(-180deg)' })),
      transition('rotated => default', animate('400ms ease-in')),
      transition('default => rotated', animate('400ms ease-in'))
    ])
  ]
})
export class PiCam7Component {
  title: string = 'Shop Camera';
  baseCamUrl: string;
  state: string = 'default';  
   
    constructor(http: HttpClient, @Inject('BASE_CAM7_URL') _baseCamUrl: string) {
      //src="http://192.168.0.62:8080/?action=stream"
      this.baseCamUrl = _baseCamUrl + '?action=stream';
      console.log('PiCam7Component::constructor() entered: camera url: ' + this.baseCamUrl);
    }

  rotate() {
    this.state = (this.state === 'default' ? 'rotated' : 'default');
  }
}

