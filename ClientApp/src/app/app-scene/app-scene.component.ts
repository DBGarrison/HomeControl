import { Component, OnInit, Input } from '@angular/core';
import { AppScenesService, AppScene, EnumSceneType } from '../appscenes/appscenes.service';

@Component({
  selector: 'app-scene',
  templateUrl: './app-scene.component.html',
  styleUrls: ['./app-scene.component.css']
})
export class AppSceneComponent implements OnInit {
  service: AppScenesService;
  @Input() appScene: AppScene;

  constructor(service: AppScenesService) {
    this.service = service;
  }

  ngOnInit() {
    console.log('AppSceneComponent.ngOnInit() entered.', this.appScene);
  }

}
