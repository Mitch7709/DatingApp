import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  private router = inject(Router);

  constructor() {
    this.CreateToastContainer();
  }

  private CreateToastContainer() {
    if (!document.getElementById('toast-container')) {
      const container = document.createElement('div');
      container.id = 'toast-container';
      container.className = 'toast toast-bottom toast-end z-50';
      document.body.appendChild(container);
    }
  }

  private createToastElement(
    message: string,
    alertClass: string,
    duration = 5000,
    avatar?: string,
    route?: string,
  ) {
    const toastcontainer = document.getElementById('toast-container');
    if (!toastcontainer) return;

    const toast = document.createElement('div');
    toast.classList.add(
      'alert',
      alertClass,
      'shadow-lg',
      'flex',
      'items-center',
      'gap-3',
      'cursor-pointer',
    );

    if (route) {
      toast.addEventListener('click', () => {
        this.router.navigateByUrl(route);
      });
    }

    if (avatar) {
      const avatarImg = document.createElement('img');
      avatarImg.src = avatar || '/user.png';
      avatarImg.classList.add('w-10', 'h-10', 'rounded');
      toast.appendChild(avatarImg);
    }

    const messageSpan = document.createElement('span');
    messageSpan.innerText = message;
    const closeButton = document.createElement('button');
    closeButton.classList.add('ml-4', 'btn', 'btn-sm', 'btn-ghost');
    closeButton.innerHTML = '&times;';
    closeButton.onclick = () => {
      toastcontainer.removeChild(toast);
    };

    toast.appendChild(messageSpan);
    toast.appendChild(closeButton);
    toastcontainer.appendChild(toast);
    setTimeout(() => {
      toastcontainer.removeChild(toast);
    }, duration);
  }

  success(message: string, duration?: number, avatar?: string, route?: string) {
    this.createToastElement(message, 'alert-success', duration, avatar, route);
  }
  error(message: string, duration?: number, avatar?: string, route?: string) {
    this.createToastElement(message, 'alert-error', duration, avatar, route);
  }
  info(message: string, duration?: number, avatar?: string, route?: string) {
    this.createToastElement(message, 'alert-info', duration, avatar, route);
  }
  warning(message: string, duration?: number, avatar?: string, route?: string) {
    this.createToastElement(message, 'alert-warning', duration, avatar, route);
  }
}
