import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Asset {
    id: number;
    name: string;
    serialNumber: string;
    status: 'InStock' | 'InUse' | 'Retired';
    createdAt: string;
}

export interface Transaction {
    id: number;
    assetId: number;
    asset?: Asset;
    userName: string;
    type: 'CheckOut' | 'CheckIn';
    date: string;
}

@Injectable({
    providedIn: 'root'
})
export class AssetService {
    private apiUrl = 'http://localhost:8080/api'; // Direct to Backend Container

    constructor(private http: HttpClient) { }

    getAssets(): Observable<Asset[]> {
        return this.http.get<Asset[]>(`${this.apiUrl}/Assets`);
    }

    createAsset(asset: any): Observable<Asset> {
        return this.http.post<Asset>(`${this.apiUrl}/Assets`, asset);
    }

    checkOut(assetId: number, userName: string): Observable<Transaction> {
        return this.http.post<Transaction>(`${this.apiUrl}/Transactions/check-out?assetId=${assetId}&userName=${userName}`, {});
    }

    checkIn(assetId: number): Observable<Transaction> {
        return this.http.post<Transaction>(`${this.apiUrl}/Transactions/check-in?assetId=${assetId}`, {});
    }

    getHistory(): Observable<Transaction[]> {
        return this.http.get<Transaction[]>(`${this.apiUrl}/Transactions`);
    }
}
