import { Component, OnInit, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { trigger, state, style, animate, transition } from '@angular/animations';

@Component({
  selector: 'app-picamera',
  templateUrl: './picamera.component.html',
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

export class PiCameraComponent {
  cameraName = 'Office Camera';
  cameraUrl: string;
  rotated = 'default';

  rotate() {
    this.rotated = (this.rotated === 'default' ? 'rotated' : 'default');
  }
}

