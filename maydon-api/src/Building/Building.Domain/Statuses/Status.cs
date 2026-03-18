namespace Building.Domain.Statuses;

public enum Status
{
	Draft = 0,                  //Черновик
	Active = 1,                 //Активный 
	Inactive,                   //Неактивный 
	Blocked,                    //Заблокирован
	Archived,                   //Архивирован
	Booked,
	Rented
}
