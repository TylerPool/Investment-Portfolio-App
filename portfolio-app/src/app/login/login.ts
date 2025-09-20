import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class Login {
  username = '';
  password = '';

  submit(): void {
    // In a real scenario this would call an auth service.
    // For now we simply log the values for debugging.
    console.log('Login attempt', { username: this.username });
  }
}
