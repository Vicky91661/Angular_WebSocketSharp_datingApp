import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ChatService } from '../chat.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [FormsModule,CommonModule],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {
  messageForm!: FormGroup;
  messages: any[] = [];
  connectedUsers: string[] = [];
  loggedInUserName = sessionStorage.getItem('user');
  AllUsers :any[]=[];

  fb = inject(FormBuilder);
  router = inject(Router);
  chatService = inject(ChatService);
  inputMessage = "";

  ngOnInit(): void {
  
    this.chatService.messages$.subscribe(messages => {
      this.messages = messages;
    });

    this.chatService.connectedUsers$.subscribe(users => {
      this.AllUsers = users;
    });
  }

  sendMessage() {
    this.chatService.sendMessage(this.inputMessage);
    this.inputMessage = '';
  }

  leaveChat() {
    this.chatService.leaveChat();
    this.router.navigate(['welcome']);
  }
}
