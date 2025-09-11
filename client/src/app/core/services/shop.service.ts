import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Pagination } from '../../shared/models/pagination';
import { Product } from '../../shared/models/product';
import { ShopParams } from '../../shared/models/shopParams';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  protected readonly baseUrl = 'https://localhost:5001/api/';
  private readonly http = inject(HttpClient);
  brands:string[] = [];
  types:string[] = [];
  getProducts(shopParams:ShopParams){
    let params = new HttpParams();
    if(shopParams.brands.length > 0){
      params = params.append('brands',shopParams.brands.join(','));
    }
    if(shopParams.types.length > 0){
      params = params.append('types',shopParams.types.join(','));
    }
    if(shopParams.sort){
      params = params.append('sort',shopParams.sort);
    }
    params = params.append('pageSize',20);
    
    return this.http.get<Pagination<Product>>(this.baseUrl + 'products',{params});
  }
  getProductTypes(){
    if(this.types.length > 0) return;
    return this.http.get<string[]>(this.baseUrl+'products/types').subscribe({
      next: response => this.types = response,
      error: error => console.log(error)
    });
  }
  getProductBrands(){
    if(this.brands.length > 0) return;
    return this.http.get<string[]>(this.baseUrl + 'products/brands').subscribe({
      next:response => this.brands = response,
      error: error => console.log(error),
    });
  }
}
