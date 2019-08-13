import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';


//CameraDashboard

@Component({
  selector: 'video-playlist',
  templateUrl: './video-playlist.component.html',
  //styles: ['./video-playlist.component.css'],
  styleUrls: ['./video-playlist.component.scss']
})

export class VideoPlaylistComponent {
  title: string = 'Video Playlist';
  videoPath: string = './20190723074653-Tower-Tower NW-rpiZero10.mp4';

  constructor() {

    console.log('VideoPlaylistComponent::constructor() entered.');
 
     
  }


}

 
