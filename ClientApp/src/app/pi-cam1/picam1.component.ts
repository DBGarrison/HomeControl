import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { trigger, state, style, animate, transition } from '@angular/animations';

@Component({
  selector: 'pi-camera1',
  templateUrl: './picam1.component.html',
  animations: [
    // Each unique animation requires its own trigger. The first argument of the trigger function is the name
    trigger('rotatedState', [
      state('default', style({ transform: 'rotate(0)' })),
      state('rotated', style({ transform: 'rotate(-180deg)' })),
      transition('rotated => default', animate('1500ms ease-out')),
      transition('default => rotated', animate('400ms ease-in'))
    ])
  ]
})
export class PiCam1Component {
  title: string = 'Rec. Room Camera';
  baseCamUrl: string;
  state: string = 'default';  
   

  constructor(http: HttpClient, @Inject('BASE_CAM1_URL') _baseCamUrl: string) {
    this.baseCamUrl = _baseCamUrl + '?action=stream';
    console.log('PiCam1Component::constructor() entered: camera url: ' + this.baseCamUrl);
  }

  rotate() {
    this.state = (this.state === 'default' ? 'rotated' : 'default');
  }
}
 
