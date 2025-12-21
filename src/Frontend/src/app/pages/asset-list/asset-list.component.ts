import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AssetService, Asset, Transaction } from '../../services/asset.service';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'app-asset-list',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './asset-list.component.html'
})
export class AssetListComponent implements OnInit {
    assets: Asset[] = [];
    history: Transaction[] = [];
    newAsset: Partial<Asset> = { name: '', serialNumber: '' };
    showHistory = false;

    constructor(public assetService: AssetService, public auth: AuthService) { }

    ngOnInit() {
        this.loadData();
    }

    loadData() {
        this.assetService.getAssets().subscribe(data => this.assets = data);
        this.assetService.getHistory().subscribe(data => this.history = data);
    }

    createAsset() {
        if (!this.newAsset.name || !this.newAsset.serialNumber) return;
        this.assetService.createAsset(this.newAsset).subscribe(() => {
            this.loadData();
            this.newAsset = { name: '', serialNumber: '' };
        });
    }

    checkOut(id: number) {
        // In real app, modal to pick user. Here we use current user.
        const user = prompt('Assign to user:', 'Employee');
        if (user) {
            this.assetService.checkOut(id, user).subscribe(() => this.loadData());
        }
    }

    checkIn(id: number) {
        if (confirm('Return this asset?')) {
            this.assetService.checkIn(id).subscribe(() => this.loadData());
        }
    }
}
