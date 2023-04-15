/*定义触发器*/
create trigger user_signup
on users
for insert,update
as
declare @_email as varchar(40)
select @_email=i.email from inserted i
if @_email not like('%@%')
begin
print'输入的邮箱格式不正确！'rollback
end
else
begin
print'插入成功！'
End

create trigger website_comfirm
on literature_website
for insert,update
as
begin
  if ((select llink from inserted) not like ('%.%'))
  begin
    print '请输入有效网址'
 rollback
  end
end



create trigger wc_1 on enword for insert as 
begin 
 declare @_en varchar(35)
 
 select @_en =en from inserted 
 
 if(@_en like('%[吖-座]%'))
  begin
   
   print '含有汉字，不能插入'
   rollback
  end
 else
  print'插入成功！'
end

create trigger rename_1
on saveexe
for insert,update
as
declare @_rename varchar(20)
declare @_username varchar(20)
declare @_num int
select @_rename=i.rename from inserted i
select @_username=i.username from inserted i
select @_num=sum(case when @_username=username and @_rename =rename then 1 else 0 end) from saveexe
if(@_num >1)
BEGIN
PRINT'不可以重复相同的软件名！'
ROLLBACK TRAN
END
