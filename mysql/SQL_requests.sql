-- ****************** func **************************
-- ************** Справочные (оперативные запросы)*****   5

-- Показать данные телеметрии пациента
select * from telemetry
where id_patients = 3;

-- Показать принимаемые препараты пациента
select pr.preparation_name as `name`, pr.preparation_doze as doze
from plan_receptions as pl
join preparations as pr on pl.id_preparation = pr.id_preparation
where pl.id_patient = 3;

-- Вывести список пациентов врача
select p.patient_first_name, p.patient_patronymic, p.patient_last_name 
from plan_receptions as pl
left join patients as p on pl.id_patient = p.id_patients
where pl.id_doctor = 3;

-- Вывести список принимаемых препаратов
select distinct pr.preparation_name from preparations as pr
inner join plan_receptions as pl on pl.id_preparation = pr.id_preparation;

-- Получить данные конкретного опекуна
select * from guardians
where id_guardians = 4;

-- *********** Справочные расчетные (аналитические запросы) ****  5
-- Показать кол-во необходимых для приема лекарств в течение дня
select sum(p_r.pils_count) as count from patients as p
join plan_receptions as p_r on p_r.id_patient = p.id_patients
where p.id_patients = 2 and p_r.reception_data_start > "2022-03-01 00:00:00";

-- Рассчитать на какой срок хватит препаратов
select c.cell_count / (p.pils_count * 1440 / (hour(p.reception_data_period) * 60 + minute(p.reception_data_period))) as days
from plan_receptions as p
join cells_to_plan_receptions as cp on cp.id_reception = p.id_reception
join cells as c on c.id_cells = cp.id_cell
where p.id_reception = 2;

-- Рассчитать время до разряда устройства
select telemetry_charge * 0.05 as `time` from telemetry
where id_patients = 3
order by telemetry_time desc
limit 1;

-- Вывести кол-во пациентов с заданным диагнозом
select count(p.id_patients) as `count` from patients_to_diagnosis as pd
join patients as p on pd.id_patients = p.id_patients
join diagnosis as d on d.icd_10_code = pd.icd_10_code
where pd.icd_10_code = "E11";

-- Рассчитать кол-во пропущенных приемов за неделю
select count(id_patients) from `events`
where id_patients = 3 and lower(event_discription) like lower("%отказ%")
and event_time like "2022-03%";

-- ******************* UPDATE *********************** 5

update diagnosis
set diagnosis_name = "Респираторная инфекция"
where icd_10_code = "J06";

update patients
set patient_phone = "7-988-123-45-78"
where id_patients = 5;

update telemetry
set telemetry_bar = 100000
where telemetry_time > "2022-03-04 16:01:00";

update guardians
set guardian_first_name = "Наталия"
where id_guardians = 1;

update patients_to_guardians
set id_guardians = 9
where id_patients = 10;

-- ****************** DELETE ************************** 5

delete from telemetry
where telemetry_time > "2022-03-09 16:01:00";

delete from guardians
where id_guardians = 10;

delete from clinics
where clinic_name = "ГУЗ Поликлиника №9";

delete from cells_to_plan_receptions
where id_cell = 10;

delete from `events`
where event_time = "2022-03-01 16:01:00" and id_patients = 10;

-- ****************** SELECT ************************** 18

select * from telemetry;

select patient_last_name, patient_patronymic, patient_first_name from patients;

select distinct id_clinic from doctors
where doctor_first_name = "Елена" or doctor_first_name = "Алина";

select id_patients from patients_to_guardians
where guardian_reletionship is null;

select id_patients from patients_to_guardians
where guardian_reletionship is null and not id_guardians = 9;

select doctor_specialization from doctors
where upper(doctor_first_name) = upper("Наталья");

select telemetry_time, id_patients from telemetry
where telemetry_charge between 30 and 70;

select telemetry_time, id_patients from telemetry
where telemetry_charge in (15, 96);

select id_patients from telemetry as t
where t.telemetry_time between "2022-03-06 16:00:00" and "2022-03-09 16:00:00";

select id_cells from cells as c
where c.cell_mass > 0.5 or c.cell_count > 5;

select distinct patient_patronymic from patients as p
where p.patient_patronymic = "Михайловна";

select fact_time from actual_receptions as a
where a.fact_num > 0 and id_reception = 8;

select * from guardians
where guardian_last_name = "Резнова";

select distinct id_patients from events as e
where e.event_time between "2022-03-01 9:00:00" and "2022-03-01 14:00:00";

select id_cell from cells_to_plan_receptions
where id_reception = 7;

select id_reception from plan_receptions as p
where p.reception_data_period is null;

select preparation_name, preparation_doze from preparations
where id_preparation = 4;

select id_cells from cells
where cell_count != 0;

-- ****************** LIKE ************************** 7

select * from events
where event_discription like ("Отказ от приема в течение%");

select * from events
where event_discription like concat("%","заправка","%");

select doctor_phone from doctors
where lower(doctor_patronymic) like lower("%ович");

select clinic_name, clinic_address from clinics
where clinic_address like "г. Волгоград, пр. Металлургов, 1_"; 

select * from preparations
where preparation_doze like "%мг";

select patient_first_name, patient_patronymic, patient_last_name from patients as p
where p.patient_last_name like "%ова";

select * from guardians
where guardian_first_name like "%а";

-- ****************** SELECT INTO ************************** 2

create table doctors_2 like doctors;

insert into doctors_2 (doctor_phone, doctor_specialization, 
doctor_first_name, doctor_patronymic, doctor_last_name, id_clinic)
select doctor_phone, doctor_specialization, 
doctor_first_name, doctor_patronymic, doctor_last_name, id_clinic
from doctors where doctor_specialization = "Эндокринолог";

create table events_2 like `events`;

insert into events_2 (event_time, id_patients, event_discription)
select *
from `events` where event_time between "2022-03-01 9:00:00" and "2022-03-01 14:00:00";

-- ****************** JOIN ************************** 15

-- врачи с названиями их клиник
select c.clinic_name, d.doctor_first_name, d.doctor_patronymic, d.doctor_last_name from doctors as d
inner join clinics as c on d.id_clinic = c.id_clinics;

-- все клиники с их врачами
select c.clinic_name, d.doctor_first_name, d.doctor_patronymic, d.doctor_last_name from clinics as c
left join doctors as d on d.id_clinic = c.id_clinics;

-- все события с ФИО пацинтов
select e.event_time, e.event_discription, p.patient_first_name, p.patient_patronymic, 
p.patient_last_name from `events` as e
right join patients as p on e.id_patients = p.id_patients; 

-- вывести пациентов и их опекунов
select p.patient_first_name, p.patient_patronymic, p.patient_last_name, 
g.guardian_first_name, g.guardian_patronymic, g.guardian_last_name from patients as p
left join patients_to_guardians as pg on pg.id_patients = p.id_patients
join guardians as g on g.id_guardians = pg.id_guardians;

-- вывести пациентов и их планы лечения
select pl.id_reception, pa.patient_first_name, pa.patient_patronymic, pa.patient_last_name 
from plan_receptions as pl
join patients as pa on pl.id_patient = pa.id_patients
where pa.patient_first_name != "Иван";

-- вывести пациентов, планы лечения и назначающего врача
select pl.id_reception, pa.patient_first_name, pa.patient_patronymic, pa.patient_last_name, 
d.doctor_first_name, d.doctor_patronymic, d.doctor_last_name
from plan_receptions as pl
left join patients as pa on pl.id_patient = pa.id_patients
join doctors as d on d.id_doctor = pl.id_doctor;

-- вывести пациентов, планы лечения, назначающего врача и клинику в которой он работает
select pl.id_reception, pa.patient_first_name, pa.patient_patronymic, pa.patient_last_name,
d.doctor_first_name, d.doctor_patronymic, d.doctor_last_name, c.clinic_name
from plan_receptions as pl
left join patients as pa on pl.id_patient = pa.id_patients
join doctors as d on d.id_doctor = pl.id_doctor
inner join clinics as c on c.id_clinics = d.id_clinic;

-- вывести пациентов, принимающих Диабетон
select pa.patient_first_name, pa.patient_patronymic, pa.patient_last_name 
from plan_receptions as pl
join patients as pa on pl.id_patient = pa.id_patients
join preparations as pr on pr.id_preparation = pl.id_preparation
where pr.preparation_name = "Диабетон";

-- вывести пациента для каждой ячейки
select c_p.id_cell, pa.patient_first_name, pa.patient_patronymic, pa.patient_last_name 
from plan_receptions as pl
join cells_to_plan_receptions as c_p on c_p.id_reception = pl.id_reception
join patients as pa on pl.id_patient = pa.id_patients;

-- вывести пациента для каждого приема таблетки
select ar.fact_num, ar.fact_time, ar.id_cell,
pa.patient_first_name, pa.patient_patronymic, pa.patient_last_name 
from actual_receptions as ar
join plan_receptions as pl on pl.id_reception = ar.id_reception
join patients as pa on pl.id_patient = pa.id_patients;

-- вывести пациентов с диагнозами
select d.diagnosis_name, pa.patient_first_name, pa.patient_patronymic, pa.patient_last_name
from patients as pa
join patients_to_diagnosis as pd on pd.id_patients = pa.id_patients
join diagnosis as d where d.icd_10_code = pd.icd_10_code;

-- вывести клинники, в которых нет врачей
select distinct c.id_clinics from clinics as c
left join doctors as d on d.id_clinic = c.id_clinics
where d.id_doctor is null;

-- вывести показания телеметрии с указанием пациента 
select t.telemetry_time, t.telemetry_charge, t.telemetry_temp, t.telemetry_bar, 
p.patient_first_name, p.patient_patronymic, p.patient_last_name from telemetry as t
join patients as p on t.id_patients = p.id_patients;

-- вывести врачей без пациентов
select distinct
d.doctor_first_name, d.doctor_patronymic, d.doctor_last_name from doctors as d
left join plan_receptions as pl on d.id_doctor = pl.id_doctor
right join patients as p on p.id_patients = pl.id_patient
where d.id_doctor is null;

-- вывести пациентов и их планы лечения
select pl.id_reception, pa.patient_first_name, pa.patient_patronymic, pa.patient_last_name 
from plan_receptions as pl
join patients as pa on pl.id_patient = pa.id_patients
where pa.patient_first_name like "%а";

-- ****************** GROUP BY and agregat *********** 15

-- сгруппировать пациентов по имени и посчитать сколько человек с таким именем
select p.patient_first_name, count(p.id_patients) from patients as p
group by p.patient_first_name;

-- сгруппировать ячейки по приемам и найти суммарное кол-во таблеток в них
select id_reception, sum(c.cell_count) from cells_to_plan_receptions as cp
join cells as c on cp.id_cell = c.id_cells
group by cp.id_reception;

-- сгруппировать телеметрию и посчитать среднюю температуру для каждого пациента
select p.patient_first_name, p.patient_patronymic, p.patient_last_name, 
avg(t.telemetry_temp) from telemetry as t
join patients as p on t.id_patients = p.id_patients
group by t.id_patients
order by p.patient_first_name, t.telemetry_temp;

-- сгруппировать докторов по именам и подсчитать сколько человек с таким именем
select doctor_first_name, count(id_doctor) from doctors
group by doctor_first_name;

insert into `events` (event_time, id_patients,event_discription) values
("2022-03-01 07:02:00", 1, "Отказ от приема в течение 17 минут");

-- сгруппировать события по пациентам и посчитать их колличество
select p.patient_first_name, p.patient_patronymic, p.patient_last_name, 
count(e.event_time) as count_event from `events` as e
join patients as p on e.id_patients = p.id_patients
group by e.id_patients;

-- сгруппировать пациентов по диагнозам и вывести их количество
select d.icd_10_code, count(pd.id_patients) as count from diagnosis as d
join patients_to_diagnosis as pd on pd.icd_10_code = d.icd_10_code
group by d.icd_10_code;

-- сгруппировать опекунов по пациентам и посчитать их количество
select pg.id_patients, count(pg.id_patients) as count from guardians as g
join patients_to_guardians as pg on pg.id_guardians = g.id_guardians
group by pg.id_patients;

-- сгруппировать события по описаниям и вывести их количество
select event_discription, count(id_patients) as count from events
group by event_discription;

-- сгруппировать опекунов по именам и подсчитать сколько человек с таким именем
select guardian_first_name, count(id_guardians) as count from guardians
group by guardian_first_name;

-- сгруппировать ячейки по приемам и найти суммарную массу таблеток в них
select id_reception, sum(c.cell_mass) as sum from cells_to_plan_receptions as cp
join cells as c on cp.id_cell = c.id_cells
group by cp.id_reception;

-- сгруппировать актуальные приемы по пациентам и найти суммарное количество принятых таблеток
select pr.id_patient, sum(ar.fact_num) as sum_pils from actual_receptions as ar
join plan_receptions as pr on pr.id_reception = ar.id_reception
group by pr.id_patient;

-- сгруппировать препараты по дозировкам и вывести их количество
select preparation_doze, count(preparation_name) as count from preparations
group by preparation_doze;

-- сгруппировать телеметрию и посчитать максимальную температуру для каждого пациента
select t.id_patients, max(t.telemetry_temp) as max_temp from telemetry as t
group by t.id_patients; 

-- сгруппировать препараты по названиям и вывести их количество
select preparation_name, count(preparation_doze) as count from preparations
group by preparation_name;

-- сгруппировать события по времени и вывести их количество
select event_time, count(id_patients) as count from events
group by event_time;


-- ****************** UNION, EXCEPT, INTERSECT *********** 3

-- вывести ФИО для пациентов и опекунов
select patient_first_name, patient_patronymic, patient_last_name from patients
union select guardian_first_name, guardian_patronymic, guardian_last_name from guardians;

-- вывести номера телефонов врачей и клиник
select doctor_phone from doctors
union select clinic_phone from clinics;

-- вывести даты всех событий и приемов
select event_time from `events`
union select fact_time from actual_receptions;

-- ****************** Вложенные SELECT с GROUP BY, ALL, ANY, EXISTS *********** 3

-- вывести показания телеметрии, где температура выше средней
select * from telemetry
where telemetry_temp > 
(select avg(telemetry_temp) from telemetry);

-- вывести все клиники, в которых есть врачи
select * from clinics	
where exists (select * from doctors where doctors.id_clinic = clinics.id_clinics);

-- вывести все события для людей, для которых присутствует хотя бы одно показание телеметрии
select * from events as e
join patients as p on p.id_patients = e.id_patients
where p.id_patients = any (select id_patients from telemetry);

-- ****************** GROUP_CONCAT *********** 3

-- вывести все имена докторов
select group_concat(doctor_first_name) from doctors;

-- вывести все существующие препараты
select group_concat(preparation_name separator ";") as "Препараты" from preparations;

-- вывести телефоны всех врачей эндокринологов
select group_concat(doctor_phone separator ";") as "Номера телефонов" from doctors 
where doctor_specialization = "Эндокринолог";

-- ****************** WITH ***********  2

-- вывести количество полученных данных телеметрии по дням в марте
with table_cte(`day`, temp) as 
(select date_format(telemetry_time, '%d') as `day`, telemetry_temp as temp
from telemetry where date_format(telemetry_time, '%m') = "03")
select `day`, count(temp) as temperature
from table_cte
group by `day`;

-- вывести количество полученных событий по дням в марте
with table_cte(`day`, discription) as
(select date_format(event_time, '%d') as `day`, event_discription as discription
from `events` where date_format(event_time, '%m') = "03")
select `day`, count(discription) as discription
from table_cte
group by `day`;

-- ****************** str, date, math *********** 6

-- вывести все события с отформатированным временем
select date_format(event_time, '%d.%m.%Y,%H:%i:%S') as `date`, 
id_patients, event_discription from `events`;

-- вывести всю информацию о пациенте, объединив ФИО
select patient_born, patient_phone,
concat_ws(" ", patient_last_name,patient_first_name,patient_patronymic) as "ФИО" 
from patients;

-- вывести кол-во символов в описании каждого события для пациента
select event_time, id_patients, length(event_discription) as "lenght" 
from `events`;

-- вывести все описания событий в верхнем регистре
select event_time, id_patients, upper(event_discription) as "lenght" 
from `events`; 

-- вывести давление по модулю 100000
select telemetry_time, id_patients, 
telemetry_bar mod 100000 as telemetry_bar from telemetry;

-- вывести температуру в кельвинах
select telemetry_time, id_patients, 
telemetry_temp + 273 as telemetry_bar from telemetry;

-- ****************** HARD *********** 5

-- Показать кол-во необходимых для приема лекарств в течение дня для каждого пациента
select p.patient_first_name, p.patient_patronymic, p.patient_last_name, count(p_r.id_reception) as count 
from patients as p
join plan_receptions as p_r on p_r.id_patient = p.id_patients
where p_r.reception_data_start > "2022-03-01 00:00:00"
group by id_patient;

-- вывести 5 первых планов лечения и пациентов
select pl.id_reception, pa.patient_last_name, pa.patient_patronymic, pa.patient_first_name 
from plan_receptions as pl
join patients as pa on pl.id_patient = pa.id_patients
order by patient_last_name asc
limit 5;

-- 3 последних события с ФИО пацинтов
select e.event_time, e.event_discription, p.patient_first_name, p.patient_patronymic, 
p.patient_last_name from `events` as e
right join patients as p on e.id_patients = p.id_patients
order by e.event_time desc
limit 3; 

-- вывести количество событий для каждого пациента
select p.patient_first_name, p.patient_patronymic, p.patient_last_name, 
count(e.event_time) as count_events from events as e
join patients as p on p.id_patients = e.id_patients
where p.id_patients = any (select id_patients from telemetry)
group by p.id_patients;

-- вывести кол-во врачей для каждой клиники
select c.clinic_name, count(d.id_doctor) as count from clinics as c
left join doctors as d 	on d.id_clinic = c.id_clinics
group by c.id_clinics;

--  ******************* mod ****************************--
-- вывести всех пациентов у которых средняя температура за последний месяц > 23 (beetween)

insert into telemetry (telemetry_time,id_patients,telemetry_temp,telemetry_bar,telemetry_charge) values
('2022.03.03 16:01:00',1,24,100123,57),
('2022.03.04 16:01:00',2,25,100456,61),
('2022.03.05 16:01:00',3,16,100247,82);

with table_cte(temp, FIO, `DATE`) as 
(select 
telemetry_temp as temp,
concat_ws(" ", p.patient_last_name,p.patient_first_name,p.patient_patronymic) as FIO,
telemetry_time as `DATE`
from telemetry as t
join patients as p on p.id_patients = t.id_patients) 
select FIO, avg(temp) as temp from table_cte
where (`DATE` between "2022.03.01 00:00:00" and "2022.03.30 23:59:59") -- 
group by FIO
having avg(temp) > 23;

-- топ 3 пациента по количеству принимаемых препаратов(фио кол-во) сортировку по кол-во
insert into plan_receptions (reception_data_start, reception_data_end, reception_data_period, 
reception_data, reception_rule, pils_count, id_preparation, id_patient, id_doctor) values
('2022.03.01 7:00:00','2022.03.02 23:00:00',null,'12:00:00','Принимать по',1,2,1,1),
('2022.03.01 7:00:00','2022.03.02 23:00:00','03:00:00',null,'Принимать по',1,3,1,2),
('2022.03.01 7:00:00','2022.03.03 23:00:00',null,'12:00:00','Принимать по',1,3,4,3);

select concat_ws(" ", p.patient_last_name,p.patient_first_name,p.patient_patronymic) as FIO, 
sum(pils_count) as `count`
from plan_receptions as pl
join patients as p on p.id_patients = pl.id_patient
group by id_patients
order by `count` desc, FIO asc
limit 3;

-- вывести пациентов, которые просрочили хотя бы один прием
select concat_ws(" ", p.patient_last_name,p.patient_first_name,p.patient_patronymic) as FIO
from `events` as e
join patients as p on e.id_patients = p.id_patients
where lower(e.event_discription) like ("%отказ%");