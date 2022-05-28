-- hard запросы
set profiling = 1;
set @@profiling = 0;
set @@profiling_history_size=0;
set @@profiling_history_size=100;
show profiles;
-- -----------------------------------------

create index index_cells on cells (cell_count);

select id_cells from cells
where cell_count > 0;

drop index index_cells on cells;

-- -----------------------------------------

create index index_preparations on preparations (preparation_name);
select pa.patient_first_name, pa.patient_patronymic, pa.patient_last_name 
from plan_receptions as pl
join patients as pa on pl.id_patient = pa.id_patients
join preparations as pr on pr.id_preparation = pl.id_preparation
where pr.preparation_name = "Acetaminophen";

drop index index_preparations on preparations;
-- -----------------------------------------
create index index_patients on patients (patient_last_name);

select patient_first_name, patient_patronymic, patient_last_name from patients
where patient_last_name like "B%";

drop index index_patients on patients;
-- -----------------------------------------
create index index_patients_to_guardians on patients_to_guardians (guardian_reletionship);

select id_patients from patients_to_guardians
where guardian_reletionship is null;

drop index index_patients_to_guardians on patients_to_guardians;

-- -----------------------------------------
create index index_telemetry on telemetry (telemetry_charge);

explain select telemetry_time, id_patients from telemetry
where telemetry_charge between 30 and 70;

drop index index_telemetry on telemetry;


show variables like 'secure_file_priv';

load data infile 'C:\\ProgramData\\MySQL\\MySQL Server 8.0\\Uploads\\cells.csv'
into table cells
fields terminated by ','
lines terminated by '\r\n'
ignore 1 lines
(cell_count, cell_mass);

load data infile 'C:\\ProgramData\\MySQL\\MySQL Server 8.0\\Uploads\\clinics_1.csv'
into table clinics
fields terminated by ','
lines terminated by '\n'
ignore 1 lines
(clinic_name, clinic_address, clinic_phone);


load data infile 'C:\\ProgramData\\MySQL\\MySQL Server 8.0\\Uploads\\doctors_1.csv'
into table doctors
fields terminated by ';'
lines terminated by '\r\n'
ignore 1 lines
(doctor_phone,doctor_specialization,doctor_first_name,doctor_patronymic,doctor_last_name,id_clinic);

load data infile 'C:\\ProgramData\\MySQL\\MySQL Server 8.0\\Uploads\\patients_1.csv'
into table patients
fields terminated by ';'
lines terminated by '\r\n'
ignore 1 lines
(patient_phone, patient_born, patient_first_name, patient_patronymic, patient_last_name);

load data infile 'C:\\ProgramData\\MySQL\\MySQL Server 8.0\\Uploads\\telemetry_1.csv'
into table telemetry
fields terminated by ','
lines terminated by '\n'
ignore 1 lines
(telemetry_time, id_patients, telemetry_temp, telemetry_bar, telemetry_charge);

load data infile 'C:\\ProgramData\\MySQL\\MySQL Server 8.0\\Uploads\\guardians_1.csv'
into table guardians
fields terminated by ';'
lines terminated by '\r\n'
ignore 1 lines
(guardian_phone, guardian_first_name, guardian_patronymic, guardian_last_name);

load data infile 'C:\\ProgramData\\MySQL\\MySQL Server 8.0\\Uploads\\patients_to_guardians.csv'
into table patients_to_guardians
fields terminated by ','
lines terminated by '\r\n'
ignore 1 lines
(id_guardians, id_patients, guardian_reletionship);


load data infile 'C:\\ProgramData\\MySQL\\MySQL Server 8.0\\Uploads\\preparations.csv'
into table preparations
fields terminated by ','
lines terminated by '\n'
ignore 1 lines
(preparation_name, preparation_doze);

load data infile 'C:\\ProgramData\\MySQL\\MySQL Server 8.0\\Uploads\\plan_receptions.csv'
into table plan_receptions
fields terminated by ';'
lines terminated by '\n'
ignore 1 lines
(reception_data_start, reception_data_end, reception_data_period, reception_data,
reception_rule, pils_count, id_preparation, id_patient, id_doctor);

load data infile 'C:\\ProgramData\\MySQL\\MySQL Server 8.0\\Uploads\\cells_to_plan_receptions.csv'
into table cells_to_plan_receptions
fields terminated by ','
lines terminated by '\r\n'
ignore 1 lines
(id_cell, id_reception);

# truncate patients_to_diagnosis;
# drop table patients_to_diagnosis; 

-- ****************** procedures ***************

# Осуществляет поиск и вывод всех пациентов с заданным именем
delimiter $$
create procedure GetPatients(in search_name varchar(255))
begin
	select patient_first_name, patient_patronymic, patient_last_name 
	from patients
	where patient_first_name like search_name;
end$$
delimiter ;

call GetPatients("Bobbi");


# Подсчитать кол-во пациентов, принимающих определенный препарат
delimiter $$
create procedure GetPatientsWithPreparation(in search_name varchar(255), out result int)
begin
	select count(pa.patient_first_name) into result 
	from plan_receptions as pl
	join patients as pa on pl.id_patient = pa.id_patients
	join preparations as pr on pr.id_preparation = pl.id_preparation
	where pr.preparation_name like search_name;
end$$
delimiter ;

set @vare := null;
call GetPatientsWithPreparation("Isopropyl Alcohol",@vare);
select @vare as GetPatientsWithPreparation;

# Вывести для пациента список его ячеек и оценить кол-во таблеток
delimiter $$
create procedure GetPatientsCellsStatus(in search_name varchar(255))
begin
	select c.id_cells,
	case
		when c.cell_count < 3 then "Мало"
		else "Достаточно"
	end as cell_count_status
	from patients as p
	join plan_receptions as p_l on p_l.id_patient = p.id_patients
	join cells_to_plan_receptions as c_p on c_p.id_reception = p_l.id_reception
	join cells as c on c.id_cells = c_p.id_cell
	where p.patient_first_name like search_name;
end$$
delimiter ;

call GetPatientsCellsStatus("Bobbi");

-- ****************** functions ***************

# Рассчитать кол-во необходимых для приема лекарств в течение дня
delimiter $$
create function GetPillsCountInDay(search_id int)
returns int
deterministic
begin
	declare res int default 0;
    select sum(p_r.pils_count) into res from patients as p
	join plan_receptions as p_r on p_r.id_patient = p.id_patients
	where p.id_patients = search_id;
return(ifnull(res, 0));
end$$
delimiter ;

-- Вывести 10 первых пациента их кол-во принимаемых ими препаратов за день
select patient_first_name, patient_patronymic, patient_last_name, 
GetPillsCountInDay(id_patients) as GetPillsCountInDay from patients
limit 10;

# Рассчитать на какой срок хватит препаратов для приема
delimiter $$
create function GetPillsEatTime(search_id int)
returns float
deterministic
begin
	declare res float default 0;
    select sum(c.cell_count / (p.pils_count * 1440 / (hour(p.reception_data_period) * 60 + minute(p.reception_data_period))))
    into res
	from plan_receptions as p
	join cells_to_plan_receptions as cp on cp.id_reception = p.id_reception
	join cells as c on c.id_cells = cp.id_cell
	where p.id_reception = search_id;
return(ifnull(res, 0));
end$$
delimiter ;

select p_l.id_reception,
p.patient_first_name, p.patient_patronymic, p.patient_last_name, 
GetPillsEatTime(p_l.id_reception) as GetPillsEatTime from patients as p
join plan_receptions as p_l on p_l.id_patient = p.id_patients
limit 20;
    
    
# Рассчитать время до разряда устройства в днях
delimiter $$
create function GetDispenserLiveTime(search_id int)
returns float
deterministic
begin
	declare res float default 0;
    select telemetry_charge * 0.05 into res from telemetry
	where id_patients = search_id
	order by telemetry_time desc
	limit 1;
return(ifnull(res, 0));
end$$
delimiter ;

select patient_first_name, patient_patronymic, patient_last_name,
GetDispenserLiveTime(id_patients) as GetDispenserLiveTime from patients
limit 20;

show procedure status where db = "tablet_dispenser";
show function status where db = "tablet_dispenser";


-- ************** view ****************

# вывод всех докторов с клиниками в которых они работают
create view DoctorsAndClinics
as 
select c.clinic_name, c.clinic_address,
d.doctor_first_name, d.doctor_patronymic, d.doctor_last_name
from doctors as d
left join clinics as c on c.id_clinics = d.id_clinic;

-- вызов представления
select * from DoctorsAndClinics;

# Вывести информацию о пациентах и их приемах
create view PatientsAndPlans
as
select p.id_patients, p.patient_first_name, p.patient_patronymic, p.patient_last_name,
p_l.id_reception, p_l.reception_data_start, p_l.reception_data_end,
p_l.reception_data_period, p_l.pils_count, pr.preparation_name, pr.preparation_doze
from patients as p
left join plan_receptions as p_l on p_l.id_patient = p.id_patients
left join preparations as pr on pr.id_preparation = p_l.id_preparation;

-- вызов представления
select * from PatientsAndPlans;

# Вывести план приема и его ячейки
create view PlanAndCells
as
select p_l.id_reception, p_l.id_patient, p_l.id_preparation, p_l.pils_count,
c.id_cells, c.cell_count, c.cell_mass 
from plan_receptions as p_l
join cells_to_plan_receptions as c_p on c_p.id_reception = p_l.id_reception
join cells as c on c.id_cells = c_p.id_cell;

select * from PlanAndCells;

-- ************* trigger **************

# Проверяем что одновременно не заполнены два поля 
# reception_data и reception_data_period
# в противном случае обнуляем reception_data
delimiter |
create trigger trigger_check_plan_receptions
before insert
on plan_receptions for each row
begin
	if not isnull(new.reception_data) and not isnull(new.reception_data_period) 
    then
		set new.reception_data = null;
    end if;
end;
|
delimiter ;

insert into plan_receptions (reception_data_start, reception_data_end, reception_data_period, 
reception_data, reception_rule, pils_count, id_preparation, id_patient, id_doctor) values
('2022.03.01 7:00:00','2022.03.02 23:00:00','12:00:00','12:00:00','Принимать по',1,1,1,1);

drop trigger trigger_check_plan_receptions;

show triggers from tablet_dispenser;


-- ************mod***********
/*1. заменить поиски в функциях на like%% в процедурах
2. Сделать процедуру чтобы отображадись люди с заданным именем(like) и возвращалось их кол-во
3. Представление выводит имя пациента и все значения заряда батареи и временем
4. Оптимизировать запрос
*/

create index index_events on `events` (event_discription);

-- вывести пациентов, которые просрочили хотя бы один прием
select concat_ws(" ", p.patient_last_name,p.patient_first_name,p.patient_patronymic) as FIO
from `events` as e
join patients as p on e.id_patients = p.id_patients
where lower(e.event_discription) like ("Отказ от приема в течение%");

drop index index_events on `events`;

# Сделать процедуру чтобы отображадись люди с заданным именем(like) и возвращалось их кол-во
delimiter $$
create procedure GetPatientAnsCount(in search_name varchar(255), out result int)
begin
	select 
    p.patient_last_name, p.patient_first_name, p.patient_patronymic
	from patients as p
	where p.patient_first_name like search_name;
    
    select count(p.patient_first_name) into result 
	from patients as p
	where p.patient_first_name like search_name;
end$$
delimiter ;

set @varee := null;
call GetPatientAnsCount("Bobbi", @varee);
select @varee as GetPatientAnsCount;

-- Вывод имени пациента и всех значений заряда батареи и времени
create view PatientBatteryTime
as
select p.patient_first_name, p.patient_patronymic, p.patient_last_name, t.telemetry_charge, t.telemetry_time from patients as p
join telemetry as t on t.id_patients = p.id_patients;

-- вызов представления
select * from PatientBatteryTime;