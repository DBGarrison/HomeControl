import { Component,  OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subscription } from 'rxjs/Rx';
import { delay } from 'q';
 
import { AppScenesService, AppScene } from '../appscenes/appscenes.service';

@Component({
  selector: 'app-scenes',
  templateUrl: './app-scenes.component.html',
  styleUrls: ['./app-scenes.component.css']
})
export class AppScenesComponent implements OnInit, OnDestroy {
  public lastUpdate = 'Unavailable'
  private sub: Subscription; 
  public appScenes: AppScene[];
  service: AppScenesService;

  constructor(_service: AppScenesService ) {
    this.service = _service;
    console.log('AppScenesComponent.constructor(): Entered');
  }

  ngOnInit() {     
    console.log('AppScenesComponent.ngOnInit() entered.');    
    this.reload();
    var ticker = Observable.timer(500, 30000);
    this.sub = ticker.subscribe(x => this.reload());
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  reload() {
    var result = this.service.getScenes();
    let cnt: number = 0;
     
    while (!result && cnt++ < 5) {
      delay(500);
      result = this.service.getScenes();
    }
    
    if (result) {       
      this.lastUpdate = this.service.lastUpdate;
    }

    this.appScenes = result;
  }

  
}

 
