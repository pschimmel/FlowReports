<?xml version="1.0"?>
<Report LastChanged="2023-03-07T23:57:14.7066115+01:00">
  <Bands>
    <Band ID="10831ead643a41e4a14a80208fa41ed5" DataSource="Company">
      <Items>
        <Item Type="TextItem" ID="4d9dcda3826741fe98f959988d19919f" X="0" Y="0" Width="100" Height="20" DataSource="[Name]" />
      </Items>
      <Bands>
        <Band ID="c94366e25a08482abb66bd37a4bc3c50" DataSource="Employees">
          <Items>
            <Item Type="TextItem" ID="4109ede15a694fabb4a1abca4df7dc16" X="31" Y="2" Width="100" Height="20" DataSource="[FirstName]" />
            <Item Type="TextItem" ID="f565026ecff64f7bb3941ecc53064410" X="134" Y="4" Width="100" Height="20" DataSource="[LastName]" />
            <Item Type="TextItem" ID="de5f822a595c4b94bf394a24e9f65c2c" X="239" Y="3" Width="100" Height="20" DataSource="[DOB]" Format="d" />
            <Item Type="BooleanItem" ID="b4bf594f9dc94ccc8f288d496f6174a2" X="349" Y="6" Width="20" Height="20" DataSource="[IsExternal]" />
            <Item Type="ImageItem" ID="c8d55948cf5549659369852f4ba097ea" X="382" Y="8" Width="100" Height="100" DataSource="[Image]" />
          </Items>
        </Band>
      </Bands>
    </Band>
  </Bands>
</Report>