create table transactions (
	t_id int auto_increment primary key not null,
    origin varchar(100),
    req_type varchar(10),
    t_time datetime default current_timestamp,
    t_type varchar(100),
    t_account varchar(100) default "",
    t_price float default -1.0,
    t_quantity int default -1,
	username varchar(50) default ""
);

create table buy_sell (
	b_id int auto_increment primary key not null,
    b_type varchar(10),
    username varchar(50),
    t_account varchar(100),
	price float check (price > 0),
    quantity int check (quantity > 0)
);

DELIMITER |
CREATE FUNCTION STOCKS_OWNED (uname VARCHAR(50), acc VARCHAR(50))
RETURNS INT READS SQL DATA
BEGIN
	DECLARE stocks_bought INT;
    DECLARE stocks_sold INT;
    DECLARE stocks_owned INT;
    
    IF (uname = "admin") THEN
		SET stocks_bought = (
			select sum(quantity) as bought from buy_sell where (username = uname and b_type = "BUY") OR (username != uname and b_type = "SELL")
		);
		SET stocks_sold = (
			select sum(quantity) as bought from buy_sell where username != uname and b_type = "BUY"
		);
    ELSE
		SET stocks_bought = (
			select sum(quantity) as bought from buy_sell where username = uname and b_type = "BUY" and t_account = acc
		);
		SET stocks_sold = (
			select sum(quantity) as sold from buy_sell where username = uname and b_type = "SELL" and t_account = acc
		);
    END IF;
    
    IF (stocks_bought IS NULL) THEN
		SET stocks_owned = 0;
    ELSEIF (stocks_sold IS NULL) THEN
		SET stocks_owned = stocks_bought;
    ELSE
		SET stocks_owned = stocks_bought - stocks_sold;
    END IF;
    
	RETURN stocks_owned;
END |
DELIMITER ;

select * from transactions;
select * from buy_sell;
