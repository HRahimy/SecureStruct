import { Component, OnInit, } from '@angular/core';
import { TodoListsClient, TodosVm } from '../web-api-client';
import { KeycloakService } from 'keycloak-angular';

@Component({
  selector: 'app-todo-component',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.scss']
})
export class TodoComponent implements OnInit {
  todos: TodosVm;
  constructor(private todoLists: TodoListsClient, protected readonly keycloak: KeycloakService) { }
  ngOnInit(): void {
    this.todoLists.getTodoLists().subscribe({
      next: (result) => {
        this.todos = result;
      }
    });
  }
}
