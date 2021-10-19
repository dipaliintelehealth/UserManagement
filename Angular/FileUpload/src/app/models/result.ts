export class ResultModel<T> {
      
    success: boolean=true;
    messages: string[] = [];
    
    constructor(public model: T) {}
                       
}