import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

interface AuditLog {
    id: number;
    entityName: string;
    action: string;
    changes: string;
    userName: string;
    timestamp: string;
}

@Component({
    selector: 'app-audit-log',
    standalone: true,
    imports: [CommonModule],
    template: `
    <div class="p-6 max-w-7xl mx-auto">
      <h1 class="text-3xl font-bold text-seb-dark mb-6">System Audit Logs</h1>
      <div class="bg-white rounded shadow overflow-hidden">
        <table class="w-full text-left">
          <thead class="bg-gray-50 text-gray-600 border-b">
            <tr>
              <th class="p-4">Time</th>
              <th class="p-4">User</th>
              <th class="p-4">Action</th>
              <th class="p-4">Entity</th>
              <th class="p-4">Details</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let log of logs" class="border-b hover:bg-gray-50">
              <td class="p-4 text-gray-500 text-sm">{{log.timestamp | date:'medium'}}</td>
              <td class="p-4 font-medium">{{log.userName}}</td>
              <td class="p-4">
                <span class="px-2 py-1 rounded text-xs font-bold bg-gray-100">{{log.action}}</span>
              </td>
              <td class="p-4">{{log.entityName}}</td>
              <td class="p-4 text-gray-600 truncate max-w-xs">{{log.changes}}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class AuditLogComponent implements OnInit {
    logs: AuditLog[] = [];

    constructor(private http: HttpClient) { }

    ngOnInit() {
        this.http.get<AuditLog[]>('http://localhost:8080/api/Audit').subscribe(data => this.logs = data);
    }
}
